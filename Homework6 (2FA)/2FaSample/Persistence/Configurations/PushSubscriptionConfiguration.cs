using _2FaSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _2FaSample.Persistence.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Endpoint)
            .IsRequired();

        builder.Property(x => x.P256dh)
            .IsRequired();

        builder.Property(x => x.Auth)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany() 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}