using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public Task AddAsync(T entity, CancellationToken ct = default) =>  _dbSet.AddAsync(entity, ct).AsTask();

        public void Delete(T entity) => _dbSet.Remove(entity);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) => await _dbSet.AsNoTracking().ToListAsync(ct);

        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => _dbSet.FindAsync(id, ct).AsTask();      

        public void Update(T entity) => _dbSet.Update(entity);
    }
}
