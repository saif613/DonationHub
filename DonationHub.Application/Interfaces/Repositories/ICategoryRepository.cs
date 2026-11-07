using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> ExistsByNameArAsync(string nameAr, CancellationToken ct = default);

        Task<bool> ExistsByNameEnAsync(string nameEn, CancellationToken ct = default);

        Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<bool> IsCategoryHasCampaign(Guid Id, CancellationToken ct = default);
    }
}
