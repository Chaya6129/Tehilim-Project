using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tehilim.Core.Entities;

namespace Tehilim.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasIdentityOptions(startValue: 100);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasMany(c => c.Books)
            .WithMany(b => b.Categories)
            .UsingEntity(j =>
            {
                j.ToTable("BookCategories");
                j.HasData(new { BooksId = 1, CategoriesId = 1 });
            });

        builder.HasData(
            new Category { Id = 1, Name = "רפואה" },
            new Category { Id = 2, Name = "הצלחה" },
            new Category { Id = 3, Name = "עילוי נשמה" },
            new Category { Id = 4, Name = "ישועה" });
    }
}