using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DonationHub.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity 
    {              
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Delete(T entity);
    }
}
