using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Tehilim.Data;
using Tehilim.Data.Context;

namespace Tehilim.Tests;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly string _connectionString;

    public PostgresFixture()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<PostgresFixture>().Build();

        var baseConnectionString = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing from User Secrets.");
        _connectionString = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = $"tehilim_tests_{Guid.NewGuid():N}"
        }.ConnectionString;
    }

    public AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_connectionString).Options);

    public async Task InitializeAsync()
    {
        await using var db = CreateContext();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
    }
}