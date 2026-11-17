using DonationHub.Application.Interfaces.Services.Model;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<Result> CreateUserAsync(Guid userId, string email, string password);
        Task<Result> AddToRoleAsync(Guid userId, string role);
        Task<IdentityUserInfo?> FindByEmailAsync(string email);
        Task<IdentityUserInfo?> FindByIdAsync(Guid userId);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
    }
}
