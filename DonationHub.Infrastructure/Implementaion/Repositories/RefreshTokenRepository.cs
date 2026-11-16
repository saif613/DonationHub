using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, DateTime utcNow, CancellationToken ct = default)
        => await _dbSet.Where(rt => rt.UserId == userId).Where(RefreshToken.IsActiveExpr(utcNow)).ToListAsync(ct);

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
             await _dbSet.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);       
    }
}
