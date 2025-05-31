using Litrater.Application.Abstractions.Data;
using Litrater.Infrastructure.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Litrater.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>()
                               ?? throw new InvalidOperationException(
                                   $"Database configuration section '{DatabaseSettings.SectionName}' is missing.");

        services.AddDbContext<LitraterDbContext>(options =>
            options.UseNpgsql(databaseSettings.ConnectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString);

        return services;
    }
}