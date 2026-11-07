using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class ProcessedStripeEventConfiguration: IEntityTypeConfiguration<ProcessedStripeEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
    {
        builder.ToTable("ProcessedStripeEvents");

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(e => e.ProcessedAt);
    }
}
