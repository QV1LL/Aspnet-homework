using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesApp.Api.Models;

namespace NotesApp.Api.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(t => t.Token)
            .IsUnique();

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.IsRevoked)
            .HasDefaultValue(false);

        builder.HasOne(t => t.User)
            .WithMany() 
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}