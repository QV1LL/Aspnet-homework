using CurrencyRateSample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace  CurrencyRateSample.Infrastructure.Persistence.Configurations;

public class PendingEmailConfiguration : IEntityTypeConfiguration<PendingEmail>
{
    public void Configure(EntityTypeBuilder<PendingEmail> builder)
    {
        builder.ToTable("pending_emails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.To)
            .HasColumnName("to")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasColumnName("subject")
            .HasMaxLength(998)
            .IsRequired();

        builder.Property(x => x.Body)
            .HasColumnName("body")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.IsSent)
            .HasColumnName("is_sent")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.RetryCount)
            .HasColumnName("retry_count")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasIndex(x => x.IsSent)
            .HasDatabaseName("ix_pending_emails_is_sent");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_pending_emails_created_at");
    }
}