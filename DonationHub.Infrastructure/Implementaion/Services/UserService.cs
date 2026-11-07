using DonationHub.Application.DTOs.Requests;
using DonationHub.Application.DTOs.Responses;
using DonationHub.Application.Exceptions;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Constants;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly TimeProvider _timeProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, TimeProvider timeProvider, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _timeProvider = timeProvider;
            _userManager = userManager;
        }

        private DateTime GetUtcNow() => _timeProvider.GetUtcNow().UtcDateTime;
        private async Task<User> GetUserAsync(Guid userId, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(userId, ct);

            if (user is null)
                throw new AppException("User.NotFound", StatusCodes.Status404NotFound);

            return user;
        }

        private async Task<UserResponseDto> MapToResponseAsync(User user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.Id.ToString());

            if (identityUser is null)
                throw new AppException("General.InternalServerError", StatusCodes.Status500InternalServerError);

            return new UserResponseDto(identityUser.Email!, user.FullName, user.PreferredLanguage, user.IsActive);
        }

        public async Task<UserResponseDto> ActivateUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await GetUserAsync(userId, ct);

            user.Activate(_currentUserService.UserId, GetUtcNow());

            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToResponseAsync(user);
        }

        public async Task<UserResponseDto> ChangeUserLanguageAsync(Guid userId, string language, CancellationToken ct = default)
        {

            var user = await GetUserAsync(userId, ct);

            user.ChangePreferredLanguage(language, GetUtcNow(), _currentUserService.UserId);

            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToResponseAsync(user);
        }

        public async Task<UserResponseDto> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await GetUserAsync(userId, ct);

            user.Deactivate(_currentUserService.UserId, GetUtcNow());

            await _unitOfWork.SaveChangesAsync(ct);

            return await MapToResponseAsync(user);
        }

        public async Task<CreateStaffResponseDto> CreateStaffAsync(CreateStaffRequestDto request, CancellationToken ct = default)
        {
            var role = request.Role.Trim();

            var isAdmin = role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase);

            var isCampaignManager = role.Equals(Roles.CampaignManager, StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isCampaignManager)
                throw new AppException("User.InvalidRole", StatusCodes.Status400BadRequest);

            role = isAdmin ? Roles.Admin : Roles.CampaignManager;

            if(await _userManager.FindByEmailAsync(request.Email) is not null)
                throw new AppException("User.EmailAlreadyExists", StatusCodes.Status409Conflict);

            var temporaryPassword = GenerateTemporaryPassword();

           return await _unitOfWork.ExcuteInTRansactionAsync(async () =>
            {                       

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(applicationUser, temporaryPassword);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code == "DuplicateUserName")|| result.Errors.Any(e => e.Code == "DuplicateEmail"))
                    throw new AppException("Auth.EmailAlreadyExists", StatusCodes.Status409Conflict);

                throw new AppException("Auth.InvalidRegistrationData", StatusCodes.Status400BadRequest);
            }
           

            var roleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(applicationUser);
                throw new AppException("Auth.FailedToAssignRole", StatusCodes.Status500InternalServerError);
            }

            var now = GetUtcNow();

            var user = User.Create(applicationUser.Id, request.FullName, request.PreferredLanguage, now, _currentUserService.UserId);

            await _userRepository.AddAsync(user, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return new CreateStaffResponseDto(user.Id, applicationUser.Email!, user.FullName, role, temporaryPassword);
            }, ct);
        }

        private static string GenerateTemporaryPassword(int length = 16)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            const string all = upper + lower + digits;

            var password = new List<char>
            {
                 GetRandomChar(upper),
                 GetRandomChar(lower),
                 GetRandomChar(digits)
            };

            while (password.Count < length)
            {
                password.Add(GetRandomChar(all));
            }

            
            for (int i = password.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);

                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password.ToArray());
        }

        private static char GetRandomChar(string characters)
        {
            return characters[RandomNumberGenerator.GetInt32(characters.Length)];
        }

        public async Task<UserResponseDto> UpdateProfileAsync(UpdateProfileRequestDto request, CancellationToken ct = default)
        {
            var newEmail = request.Email!.Trim();

            var userId = _currentUserService.UserId;

            if (userId is null)
                throw new AppException("Auth.Unauthenticated", StatusCodes.Status401Unauthorized);

            var user = await _userRepository.GetByIdAsync(userId.Value, ct);

            if (user is null)
                throw new AppException("User.NotFound", StatusCodes.Status404NotFound);

            var applicationUser = await _userManager.FindByIdAsync(userId.Value.ToString());

            if (applicationUser is null)
                throw new AppException("Auth.UserNotFound", StatusCodes.Status404NotFound);

            await _unitOfWork.ExcuteInTRansactionAsync(async () =>
            {
                if(!string.Equals(applicationUser.Email, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var exsitingUser = await _userManager.FindByEmailAsync(newEmail);

                    if(exsitingUser is not null && exsitingUser.Id != userId.Value)
                       throw new AppException("Auth.EmailAlreadyExists", StatusCodes.Status409Conflict);

                    var result = await _userManager.SetEmailAsync(applicationUser, newEmail);

                    if (!result.Succeeded)
                          throw new AppException("Auth.EmailUpdateFailed", StatusCodes.Status400BadRequest);

                     var userNameResult = await _userManager.SetUserNameAsync(applicationUser, newEmail);

                     if(!userNameResult.Succeeded)
                         throw new AppException("Auth.UserNameUpdateFailed", StatusCodes.Status400BadRequest);
                }
                

                user.UpdateProfile(request.FullName, GetUtcNow(), userId.Value);

                await _unitOfWork.SaveChangesAsync(ct);
            }, ct);

            return new UserResponseDto(applicationUser.Email!, user.FullName, user.PreferredLanguage, user.IsActive);

        }
    }
}
