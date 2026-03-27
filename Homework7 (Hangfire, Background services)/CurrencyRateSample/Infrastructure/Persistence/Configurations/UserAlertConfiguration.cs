using CurrencyRateSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CurrencyRateSample.Infrastructure.Persistence.Configurations;

public class UserAlertConfiguration : IEntityTypeConfiguration<UserAlert>
{
    public void Configure(EntityTypeBuilder<UserAlert> builder)
    {
        builder.HasKey(ua => ua.Id);

        var rateConverter = new ValueConverter<decimal?, long?>(
            v => v == null ? null : (long?)Math.Round(v.Value * 10000m, 0, MidpointRounding.AwayFromZero),
            v => v / 10000m
        );

        builder.Property(ua => ua.TargetRate)
            .HasConversion(rateConverter)
            .HasColumnType("INTEGER");

        builder.Property(ua => ua.CurrencyCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasOne(ua => ua.User)
            .WithMany() 
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ua => ua.IsActive)
            .HasDefaultValue(true);
    }
}