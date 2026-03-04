using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Infrastructure.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Content)
            .HasMaxLength(3000);
        
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);
        
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Slugs)
            .HasColumnType("text[]");

        builder.HasMany(p => p.Categories)
            .WithMany(p => p.Posts)
            .UsingEntity("CategoriesPosts");
    }
}