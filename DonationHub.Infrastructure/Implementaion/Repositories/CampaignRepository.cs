using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(AppDbContext context) : base(context) { }

        public Task<bool> ExistsByTitleArAsync(string titleAr, Guid? excludeId = null, CancellationToken ct = default) => _dbSet.AsNoTracking().AnyAsync(c => c.TitleAr == titleAr && (!excludeId.HasValue || c.Id != excludeId.Value), ct);

        public Task<bool> ExistsByTitleEnAsync(string titleEn, Guid? excludeId = null, CancellationToken ct = default) => _dbSet.AsNoTracking().AnyAsync(c => c.TitleEn == titleEn && (!excludeId.HasValue || c.Id != excludeId.Value), ct);

        public void SetOriginalRowVersion(Campaign campaign, byte[] rowVersion) 
        {
            _context.Entry(campaign).Property(x => x.RowVersion).OriginalValue = rowVersion;
        }
        public async Task<bool> IsCampaignHasDonation(Guid Id, CancellationToken ct = default) => await _context.Donations.AnyAsync(c => c.CampaignId == Id, ct);

        public async Task<bool> HardDeleteAsync(Guid id, CancellationToken ct = default)
        {
            var affectedRows = await _context.Campaigns.Where(c => c.Id == id).ExecuteDeleteAsync(ct);

            return affectedRows > 0;
        }

        public async Task<IReadOnlyList<Campaign>> GetAllActiveAsync(CancellationToken ct = default) => await _dbSet
                .AsNoTracking()
                .Where(c => c.Status == CampaignStatus.Active)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Campaign>> GetAllCompletedAsync(CancellationToken ct = default) => await _dbSet
                .AsNoTracking()
                .Where(c => c.Status == CampaignStatus.Completed)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);

    }
}
