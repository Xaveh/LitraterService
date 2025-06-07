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
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryHandlerValidationDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggingDecorator<,>));

        services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerValidationDecorator<>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandHandlerLoggingDecorator<>));

        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandHandlerValidationDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(CommandHandlerLoggingDecorator<,>));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}