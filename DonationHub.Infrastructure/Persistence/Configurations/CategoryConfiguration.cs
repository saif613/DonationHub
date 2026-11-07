using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration: IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
           .ValueGeneratedOnAdd();

        builder.Property(c => c.NameAr)
            .HasMaxLength(150)
            .IsRequired();
  

        builder.Property(c => c.NameEn)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.DescriptionAr)
            .HasMaxLength(1000);

        builder.Property(c => c.DescriptionEn)
            .HasMaxLength(1000);

        builder.HasIndex(c => c.NameEn);
    }
}
