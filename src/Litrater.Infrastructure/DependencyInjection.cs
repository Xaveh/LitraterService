using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Infrastructure.Authentication;
using Litrater.Infrastructure.Authors;
using Litrater.Infrastructure.Books;
using Litrater.Infrastructure.Data;
using Litrater.Infrastructure.Users;
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
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Authentication
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenProvider, TokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHealthChecks()
            .AddNpgSql(databaseSettings.ConnectionString);

        return services;
    }
}