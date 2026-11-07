using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Implementaion.Repositories;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Implementaion.UnitOfWorK
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;
 
        public UnitOfWork(AppDbContext context)
        {
            _context = context;  
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
        

        public async Task<T> ExcuteInTRansactionAsync<T>(Func<Task<T>> action, CancellationToken ct = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var result = await action();

            await transaction.CommitAsync(ct);

            return result;
        }

        public async Task ExcuteInTRansactionAsync(Func<Task> action, CancellationToken ct = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            await action();

            await transaction.CommitAsync(ct);
        }
    }
}

