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

    public Task InitializeAsync() => DbContainer.StartAsync();

    public Task DisposeAsync() => DbContainer.DisposeAsync().AsTask();
}