using Litrater.Infrastructure.Data;

namespace Litrater.Presentation.Extensions;

internal static class DatabaseExtensions
{
    public static async Task EnsureDatabaseSeededAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LitraterDbContext>();

        await DatabaseSeeder.SeedDataAsync(context);
    }
}
