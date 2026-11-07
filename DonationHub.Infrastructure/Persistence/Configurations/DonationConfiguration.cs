using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class DonationConfiguration: IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("Donations");

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();


        builder.HasKey(d => d.Id);

        builder.OwnsOne(d => d.Amount, money =>
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
        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne<Campaign>()
            .WithMany()
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(d => d.DonorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.CampaignId);
        builder.HasIndex(d => d.DonorUserId);
        builder.HasIndex(d => d.Status);
    }
}
