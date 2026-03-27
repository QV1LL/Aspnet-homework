using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CurrencyRateSample.Models;

namespace CurrencyRateSample.Infrastructure.Persistence.Configurations;

public class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(cr => cr.Rate)
            .HasConversion<double>(); 

        builder.Property(cr => cr.LastUpdated)
            .IsRequired();
    }
}