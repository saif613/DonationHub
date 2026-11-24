using DonationHub.Application.Events;
using DonationHub.Application.Exceptions;
using DonationHub.Application.Interfaces.Repositories;
using DonationHub.Application.Interfaces.UnitOfWork;
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Implementaion.Repositories;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public UnitOfWork(AppDbContext context, IDomainEventDispatcher domainEventDispatcher)
        {
            _context = context;  
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) 
        {
            try
            {
                var entities = _context.ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(e => e.Entity.DomainEvents.Any())
                    .Select(e => e.Entity)
                    .ToList();

                var domainEvents = entities
                    .SelectMany(e => e.DomainEvents)
                    .ToList();

                var result = await _context.SaveChangesAsync(ct);

                await _domainEventDispatcher.DispatchAsync(domainEvents, ct);

                foreach (var entiry in entities)
                    entiry.ClearDomainEvents();

                return result;
            }

            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException(ex);
            }
        } 
        

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

