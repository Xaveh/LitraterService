using FluentValidation;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Litrater.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}