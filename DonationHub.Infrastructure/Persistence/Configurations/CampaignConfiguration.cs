using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CampaignConfiguration: IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();        

        builder.Property(c => c.TitleAr)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.TitleEn)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(3000)
            .IsRequired();

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(3000)
            .IsRequired();

        builder.OwnsOne(c => c.GoalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("GoalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(c => c.RaisedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CategoryId);
        builder.HasIndex(c => c.CreatedBy);
        builder.HasIndex(c => c.Status);
    }
}
