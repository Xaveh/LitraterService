using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Common;
using Litrater.Application.Abstractions.Data;
using Litrater.Infrastructure.Authors;
using Litrater.Infrastructure.Books;
using Litrater.Infrastructure.Common;
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
        var connectionString = configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException("Database connection string is missing.");

        services.AddDbContext<LitraterDbContext>(options => options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable("__EFMigrationsHistory", LitraterDbContext.DefaultSchema)));

        services.AddScoped<IBookQueryRepository, BookQueryRepository>();
        services.AddScoped<IBookCommandRepository, BookCommandRepository>();
        services.AddScoped<IBookReviewQueryRepository, BookReviewQueryRepository>();
        services.AddScoped<IBookReviewCommandRepository, BookReviewCommandRepository>();
        services.AddScoped<IAuthorQueryRepository, AuthorQueryRepository>();
        services.AddScoped<IAuthorCommandRepository, AuthorCommandRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }
}