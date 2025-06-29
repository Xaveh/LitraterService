using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.PostgreSql;

namespace Litrater.Presentation.IntegrationTests.Common;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer DbContainer { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("LitraterTestDb")
        .WithUsername("LitraterTest")
        .WithPassword("LitraterTest1234")
        .WithCleanUp(true)
        .Build();

    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();
        await MigrateDatabaseAsync();
        await CreateRespawnerAsync();
    }

    public Task DisposeAsync() => DbContainer.DisposeAsync().AsTask();

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new Npgsql.NpgsqlConnection(DbContainer.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    private async Task MigrateDatabaseAsync()
    {
        await using var dbContext = new LitraterDbContext(
            new DbContextOptionsBuilder<LitraterDbContext>().UseNpgsql(DbContainer.GetConnectionString()).Options);
        await dbContext.Database.MigrateAsync();
    }

    private async Task CreateRespawnerAsync()
    {
        await using var connection = new Npgsql.NpgsqlConnection(DbContainer.GetConnectionString());
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
}