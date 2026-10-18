using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tehilim.Core.Entities;
using Tehilim.Core.Enums;

namespace Tehilim.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private const string DemoPasswordHash =
    "AQAAAAIAAYagAAAAEGO1sYZjxGax9rbvDux+fNfEgQaftHPl3mDIiWz/UJlkI91IXm7jpRpWJOpljtNskQ==";

    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);

        builder.Property(u => u.Role)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Id).HasIdentityOptions(startValue: 100);

        builder.HasData(
            new User
            {
                Id = 1,
                DisplayName = "מנהל המערכת",
                Email = "admin@tehilim.com",
                PasswordHash = DemoPasswordHash,
                Role = UserRole.Admin,
                CreatedAt = SeedDate
            },
            new User
            {
                Id = 2,
                DisplayName = "משתמש לדוגמה",
                Email = "user@tehilim.com",
                PasswordHash = DemoPasswordHash,
                Role = UserRole.User,
                CreatedAt = SeedDate
            });
    }
}