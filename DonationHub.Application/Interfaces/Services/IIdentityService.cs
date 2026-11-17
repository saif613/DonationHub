using DonationHub.Application.Interfaces.Services.Model;
using DonationHub.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<Result> CreateUserAsync(Guid userId, string email, string password, CancellationToken ct = default);
        Task<Result> AddToRoleAsync(Guid userId, string role, CancellationToken ct = default);
        Task<IdentityUserInfo?> FindByEmailAsync(string email, CancellationToken ct = default);
        Task<IdentityUserInfo?> FindByIdAsync(Guid userId, CancellationToken ct = default);
        Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken ct = default);
        Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken ct = default);
        Task<Result> UpdateEmailAsync(Guid userId, string newEmail, CancellationToken ct = default);
     }
}
