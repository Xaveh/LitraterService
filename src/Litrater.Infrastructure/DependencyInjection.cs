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
        string connectionString = configuration.GetConnectionString("Database")
                                  ?? throw new InvalidOperationException($"Database connection string is missing.");

        services.AddDbContext<LitraterDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookReviewRepository, BookReviewRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Authentication
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenProvider, TokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }
}