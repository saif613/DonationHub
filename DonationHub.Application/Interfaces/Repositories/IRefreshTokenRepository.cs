using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

        Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, DateTime utcNow, CancellationToken ct = default);
    }
}
