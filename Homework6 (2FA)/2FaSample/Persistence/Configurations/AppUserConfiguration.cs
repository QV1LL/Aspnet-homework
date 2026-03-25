using System.Text.Json;
using _2FaSample.Enums;
using _2FaSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _2FaSample.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.EnabledTwoFactorMethods)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrEmpty(v) 
                    ? new List<TwoFactorMethod>() 
                    : JsonSerializer.Deserialize<List<TwoFactorMethod>>(v, (JsonSerializerOptions?)null) ?? new List<TwoFactorMethod>()
            )
            .HasColumnType("TEXT");
    }
}