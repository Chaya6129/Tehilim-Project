using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;

namespace Tehilim.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Purpose).IsRequired().HasMaxLength(200);
        builder.Property(b => b.IsPublic).IsRequired();

        builder.Property(b => b.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(b => b.OwnerId).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();

        builder.HasOne(b => b.Owner)
       .WithMany(u => u.OwnedBooks)
       .HasForeignKey(b => b.OwnerId)
       .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(b => new { b.Status, b.IsPublic });
        builder.Property(b => b.Id).HasIdentityOptions(startValue: 100);

        builder.HasData(new Book
        {
            Id = 1,
            Purpose = "לרפואת כל חולי ישראל",
            IsPublic = true,
            Status = BookStatus.Open,
            OwnerId = 2,
            CreatedAt = SeedDate,
            CompletedAt = null
        });
    }
}