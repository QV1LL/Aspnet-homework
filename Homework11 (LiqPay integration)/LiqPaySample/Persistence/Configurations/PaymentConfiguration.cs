using LiqPaySample.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiqPaySample.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.HasIndex(p => p.IdempotencyKey)
            .IsUnique();

        builder.Property(p => p.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>();
    }
}