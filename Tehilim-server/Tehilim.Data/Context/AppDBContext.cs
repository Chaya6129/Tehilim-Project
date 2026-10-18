using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Tehilim.Core.Entities;

namespace Tehilim.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    public override Task<int> SaveChangesAsync(
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default)
    {
        RenewChapterVersions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        RenewChapterVersions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void RenewChapterVersions()
    {
        foreach (var entry in ChangeTracker.Entries<Chapter>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.Version = Guid.NewGuid();
        }
    }
}