using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotesApp.Api.Models;

namespace NotesApp.Api.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Password)
            .IsRequired();

        builder.HasMany<Note>()
            .WithOne(n => n.Owner)
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}