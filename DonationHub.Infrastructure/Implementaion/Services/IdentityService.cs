using DonationHub.Application.Exceptions;
using DonationHub.Application.Features.DTOs.Requests;
using DonationHub.Application.Interfaces.Services;
using DonationHub.Application.Interfaces.Services.Model;
using DonationHub.Domain.Common.Results;
using DonationHub.Domain.Constants;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result> AddToRoleAsync(Guid userId, string role)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return Errors.Auth.UserNotFound; 

            var roleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!roleResult.Succeeded)
                return Errors.Auth.FailedToAssignRole;

            return Result.Success();            

        }

        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return Errors.Auth.UserNotFound;

            var result = await _userManager.ChangePasswordAsync(applicationUser, currentPassword, newPassword);

            if (!result.Succeeded)
                return Errors.Auth.PasswordChangeFailed;

            return Result.Success();
        }

        public async Task<bool> CheckPasswordAsync(Guid userId, string password)
        {
            var applicationUser = await _userManager.FindByIdAsync(userId.ToString());

            if (applicationUser is null)
                return false;

            return await _userManager.CheckPasswordAsync(applicationUser, password);            
        }

        public async Task<Result> CreateUserAsync(Guid userId, string email, string password)
        {
            var applicationUser = new ApplicationUser
            {
                Id = userId,
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(applicationUser, password);

            if (result.Succeeded)
                return Result.Success();

            if (result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
                return Errors.Auth.EmailAlreadyExists;

            return Errors.Auth.InvalidCredentials;
        }

        public async Task<IdentityUserInfo?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return null;

            return new IdentityUserInfo(user.Id, user.Email!, user.UserName!);
        }

        public async Task<IdentityUserInfo?> FindByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return null;

            return new IdentityUserInfo(user.Id, user.Email!, user.UserName!);
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Array.Empty<string>();

            return (await _userManager.GetRolesAsync(user)).ToArray();
        }
    }
}
