using Microsoft.EntityFrameworkCore;
using NotesApp.Models;

namespace NotesApp.Persistence;

public class NotesAppContext(DbContextOptions<NotesAppContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotesAppContext).Assembly);
    }
}