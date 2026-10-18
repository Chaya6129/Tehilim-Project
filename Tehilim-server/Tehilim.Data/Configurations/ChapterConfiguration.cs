using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;

namespace Tehilim.Data.Configurations;

public class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{

    public void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("Chapters");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.BookId).IsRequired();
        builder.Property(c => c.Number).IsRequired();

        builder.Property(c => c.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(c => c.TakenByName).HasMaxLength(100);

        builder.Property(c => c.Version)
               .IsRequired()
               .IsConcurrencyToken();
        builder.HasOne(c => c.Book)
       .WithMany(b => b.Chapters)
       .HasForeignKey(c => c.BookId)
       .OnDelete(DeleteBehavior.Cascade);

builder.HasOne(c => c.TakenByUser)
       .WithMany(u => u.TakenChapters)
       .HasForeignKey(c => c.TakenByUserId)
       .IsRequired(false)
       .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Book)
       .WithMany(b => b.Chapters)
       .HasForeignKey(c => c.BookId)
       .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TakenByUser)
               .WithMany(u => u.TakenChapters)
               .HasForeignKey(c => c.TakenByUserId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(c => new { c.BookId, c.Number }).IsUnique();
        builder.Property(c => c.Id).HasIdentityOptions(startValue: 1000);

        builder.HasData(Enumerable.Range(1, 150).Select(n => new Chapter
        {
            Id = n,
            BookId = 1,
            Number = n,
            Status = ChapterStatus.Available,
            Version = new Guid($"00000000-0000-0000-0000-{n:D12}")
        }));
    }
}