using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class PaymentAttemptConfiguration: IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.OwnsOne(c => c.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

      builder.Navigation(p => p.Amount)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.FailureCode)
            .HasMaxLength(100);

        builder.HasOne<Donation>()
            .WithMany()
            .HasForeignKey(p => p.DonationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.DonationId);

        builder.HasIndex(p => p.StripeCheckoutSessionId)
            .IsUnique();
    }
}
