using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace DonationHub.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork 
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task<T> ExcuteInTRansactionAsync<T>(Func<Task<T>> action, CancellationToken ct = default);

        Task ExcuteInTRansactionAsync(Func<Task> action, CancellationToken ct = default);
    }
}
