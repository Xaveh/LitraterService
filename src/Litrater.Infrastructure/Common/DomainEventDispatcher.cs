using Litrater.Application.Abstractions.DomainEvents;
using Litrater.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Litrater.Infrastructure.Common;

internal sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = serviceProvider.GetServices(handlerType);
        var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<>.Handle))!;

        foreach (var handler in handlers.OfType<object>())
        {
            await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}