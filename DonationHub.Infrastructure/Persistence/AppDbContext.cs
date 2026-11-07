using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Persistence.Configurations;
using DonationHub.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence;

public class AppDbContext: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){ }


    public DbSet<User> User => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
    public DbSet<ProcessedStripeEvent> ProcessedStripeEvents => Set<ProcessedStripeEvent>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
 
        modelBuilder.Entity<Campaign>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Donation>()
            .HasQueryFilter(d => !d.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.IsDeleted);

        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(rt => !rt.IsDeleted);

        modelBuilder.Entity<IdempotencyRecord>()
            .HasQueryFilter(i => !i.IsDeleted);

        modelBuilder.Entity<PaymentAttempt>()
            .HasQueryFilter(pa => !pa.IsDeleted);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
