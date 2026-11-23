using DonationHub.Application.Features.Auth.DTOs.Response;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Application.Options;
using DonationHub.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = DonationHub.Domain.Entities.RefreshToken;

namespace DonationHub.Application.Features.Auth.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<TokenResponseDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IIdentityService _identityService;


    public LoginHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtOptions> jwtOptions, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtOptions = jwtOptions;
        _identityService = identityService;
    }

    private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
    public async Task<Result<TokenResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {       
        var applicationUser = await _identityService.FindByEmailAsync(request.Email, ct);

        if (applicationUser is null)
            return Errors.Auth.InvalidCredentials;

        var userId = applicationUser.Id;        
        
        var passwordValid = await _identityService.CheckPasswordAsync(userId, request.Password, ct);

        if (!passwordValid)
            return Errors.Auth.InvalidCredentials;

        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user is null)
            return Errors.Auth.UserNotFound;

        if (!user.IsActive)
            return Errors.Auth.AccountDeactivated;

        var roles = await _identityService.GetRolesAsync(userId, ct);

        var now = GetUtcNow();

        var accessTokenExpiresAt = now.AddMinutes(_jwtOptions.Value.ExpiryMinutes);

        var accessToken = _jwtTokenService.GenerateJwtToken(userId, applicationUser.Email, applicationUser.UserName, user.FullName, roles);

        var refreshToken = Helpers.RefreshTokenHelper.Generate();

        var refreshTokenHash = Helpers.RefreshTokenHelper.Hash(refreshToken);

        var refreshTokenEntity = RefreshTokenEntity.Create(
            userId,
            refreshTokenHash,
            now,
            now.AddDays(_jwtOptions.Value.RefreshTokenExpiryDays),
            null
        );

        if (refreshTokenEntity.IsFailure)
            return refreshTokenEntity.Error;

        await _refreshTokenRepository.AddAsync(refreshTokenEntity.Value, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var response = new TokenResponseDto(accessToken, refreshToken, accessTokenExpiresAt, refreshTokenEntity.Value.ExpiresAt);
        return Result.Success(response);
    }
}
