using Homework10__Htmx_.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework10__Htmx_.Persistence;

public class ContactsAppContext(DbContextOptions<ContactsAppContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactsAppContext).Assembly);
    }
}