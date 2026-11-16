using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Azure.Core.HttpHeader;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class CategoryRepository: GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public Task<bool> ExistsByNameArAsync(string nameAr, Guid? excludeId = null, CancellationToken ct = default) => _dbSet.AsNoTracking().AnyAsync(c => c.NameAr == nameAr && (!excludeId.HasValue || c.Id != excludeId.Value), ct);

        public Task<bool> ExistsByNameEnAsync(string nameEn, Guid? excludeId = null, CancellationToken ct = default) => _dbSet.AsNoTracking().AnyAsync(c => c.NameEn == nameEn &&(!excludeId.HasValue || c.Id != excludeId.Value), ct);

        public async Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Categories.AsNoTracking();

            var total = await query.CountAsync(ct);

            var items = await query
                .AsNoTracking()
                .OrderBy(c => c.NameEn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<bool> IsCategoryHasCampaign(Guid Id, CancellationToken ct = default) => await _context.Campaigns.AnyAsync(c => c.CategoryId == Id, ct);
    }
}
