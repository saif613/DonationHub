using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Repositories
{
    public interface ICampaignRepository: IGenericRepository<Campaign>
    {
        public Task<bool> ExistsByTitleArAsync(string titleAr, Guid? excludeId = null, CancellationToken ct = default);
        public Task<bool> ExistsByTitleEnAsync(string titleEn, Guid? excludeId = null, CancellationToken ct = default);
        public void SetOriginalRowVersion(Campaign campaign, byte[] rowVersion);
        public Task<bool> IsCampaignHasDonation(Guid Id, CancellationToken ct = default);
        Task<bool> HardDeleteAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Campaign>> GetAllActiveAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Campaign>> GetAllCompletedAsync(CancellationToken ct = default);
        Task<bool> IsCampaignActive(Guid id, CancellationToken ct = default); 
    }
}
