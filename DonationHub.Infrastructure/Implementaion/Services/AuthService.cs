using Azure.Core;
using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.DTOs.Requests;
using DonationHub.Application.Features.DTOs.Responses;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ICurrentUserService _currentUser;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IOptions<JwtOptions> _jwtOptions;


        public AuthService(UserManager<ApplicationUser> userManager, IUserRepository userRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider, ICurrentUserService currentUser, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _currentUser = currentUser;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtOptions = jwtOptions;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        public async Task<CurrentUserResponseDto> GetCurrentUserAsync(CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;

            if (userId == null)            
                throw new AppException("Auth.UnAuthenticated", StatusCodes.Status401Unauthorized);

            var user = await _userRepository.GetByIdAsync(userId.Value, ct);

            if (user == null)
                throw new AppException("Auth.UserNotFound", StatusCodes.Status404NotFound);

            var applicationUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if (applicationUser is null)
                throw new AppException("Auth.ApplicationUserNotFound", StatusCodes.Status404NotFound);

            var roles = await _userManager.GetRolesAsync(applicationUser);

            return new CurrentUserResponseDto(
                user.Id,
                applicationUser.Email!,
                user.FullName,
                user.PreferredLanguage,
                roles.ToList()
            );
        }

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken ct = default)
        {
            var applicationUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            if(applicationUser is null)
                throw new AppException("Auth.InvalidCredentials", StatusCodes.Status401Unauthorized);

            var user = await _userRepository.GetByIdAsync(applicationUser.Id, ct);

            if (user is null)
                throw new AppException("Auth.InvalidCredentials", StatusCodes.Status401Unauthorized);

            var passwordValid = await _userManager.CheckPasswordAsync(applicationUser, loginRequestDto.Password);

            if (!passwordValid)
                throw new AppException("Auth.InvalidCredentials", StatusCodes.Status401Unauthorized);

            if (!user.IsActive)
                throw new AppException("Auth.AccountDeActive", StatusCodes.Status403Forbidden);

            var roles = (await _userManager.GetRolesAsync(applicationUser)).ToList();

            var now = GetUtcNow();

            var accessTokenExpiresAt = now.AddMinutes(_jwtOptions.Value.ExpiryMinutes);

            var accessToken = _jwtTokenService.GenerateJwtToken(applicationUser.Id, applicationUser.Email!, applicationUser.UserName!, user.FullName, roles);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenHash = HashRefreshToken(refreshToken);

            var refreshTokenEntity = RefreshToken.Create(
                applicationUser.Id,
                refreshTokenHash,
                now,
                now.AddDays(_jwtOptions.Value.RefreshTokenExpiryDays),
                _currentUser.UserId
            );

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return new TokenResponseDto
            {
                Success = true,
                Message = "Login successful.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt
            };
        }

        public async Task LogoutAsync(LogoutRequestDto logoutRequestDto, CancellationToken ct = default)
        {
            var tokenHash = HashRefreshToken(logoutRequestDto.RefreshToken);

            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

            var utcNow = GetUtcNow();

            if (refreshToken is null || !refreshToken.IsActive(utcNow))
                throw new AppException("Auth.InvalidRefreshToken", StatusCodes.Status401Unauthorized);

            refreshToken.Revoke(utcNow, _currentUser.UserId);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task<TokenResponseDto> RefreshAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken ct = default)
        {
            var tokenHash = HashRefreshToken(refreshTokenRequestDto.RefreshToken);

            var oldRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

            var utcNow = GetUtcNow();

            if (oldRefreshToken is null || !oldRefreshToken.IsActive(utcNow))
                throw new AppException("Auth.InvalidRefreshToken", StatusCodes.Status401Unauthorized);

            var user = await _userRepository.GetByIdAsync(oldRefreshToken.UserId, ct);

            if(user is null)
                throw new AppException("Auth.RefreshTokenInvalid", StatusCodes.Status401Unauthorized);

            if (!user.IsActive)
                throw new AppException("Auth.AccountDeActive", StatusCodes.Status403Forbidden);

            var applicationUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if (applicationUser is null)
                throw new AppException("Auth.RefreshTokenInvalid", StatusCodes.Status401Unauthorized);

            var roles = (await _userManager.GetRolesAsync(applicationUser)).ToList();

            var accessTokenExpiresAt = utcNow.AddMinutes(_jwtOptions.Value.ExpiryMinutes);

            var accessToken = _jwtTokenService.GenerateJwtToken(applicationUser.Id, applicationUser.Email!, applicationUser.UserName!, user.FullName, roles);

            var newRefreshToken = GenerateRefreshToken();

            var newRefreshTokenHash = HashRefreshToken(newRefreshToken);

            var newRefreshTokenEntity = RefreshToken.Create(
                applicationUser.Id,
                newRefreshTokenHash,
                utcNow,
                utcNow.AddDays(_jwtOptions.Value.RefreshTokenExpiryDays),
                _currentUser.UserId
            );

            oldRefreshToken.ReplaceWith(newRefreshTokenEntity.Id, utcNow, _currentUser.UserId);

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return new TokenResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully.",
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = newRefreshTokenEntity.ExpiresAt
            };
        }

        public async Task RegisterAsync(RegisterRequestDto registerRequestDto, CancellationToken ct = default)
        {
            var now = GetUtcNow();

            if (await _userManager.FindByEmailAsync(registerRequestDto.Email) is not null)
                throw new AppException("Auth.EmailAlreadyExists", StatusCodes.Status409Conflict);
            await _unitOfWork.ExcuteInTRansactionAsync(async () =>
            {

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email
            };


            var result = await _userManager.CreateAsync(applicationUser, registerRequestDto.Password);

            if (!result.Succeeded)
            {
               if(result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
                throw new AppException("Auth.EmailAlreadyExists", StatusCodes.Status409Conflict);

               throw new AppException("Auth.InvalidRegistrationData", StatusCodes.Status400BadRequest);
            }

                var roleResult = await _userManager.AddToRoleAsync(applicationUser, Roles.Donor);

                if (!roleResult.Succeeded)
                {
                    throw new AppException("Auth.FailedToAssignRole", StatusCodes.Status500InternalServerError);
                }

                var user = User.Create(applicationUser.Id, registerRequestDto.FullName, registerRequestDto.PreferredLanguage, now, _currentUser.UserId);
                        
                await _userRepository.AddAsync(user, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            },ct);            
        }

        private static string GenerateRefreshToken()
        {
            var random = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(random);
        }

        private static string HashRefreshToken(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDto request, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;

            if (userId is null)
                throw new AppException("Auth.Unauthenticated", StatusCodes.Status401Unauthorized);

            var applicationUser = await _userManager.FindByIdAsync(userId.Value.ToString());

            if (applicationUser is null)
                throw new AppException("Auth.UserNotFound", StatusCodes.Status404NotFound);


            await _unitOfWork.ExcuteInTRansactionAsync(async () =>
            {
           
            var result = await _userManager.ChangePasswordAsync(applicationUser, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                throw new AppException("Auth.PasswordChangeFailed", StatusCodes.Status400BadRequest);

            var now = GetUtcNow();

            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId.Value, now, ct);

            foreach (var token in activeTokens)
                token.Revoke(now, userId);

            await _unitOfWork.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
