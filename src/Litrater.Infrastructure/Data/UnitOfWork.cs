using Litrater.Application.Abstractions.Common;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Abstractions.DomainEvents;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

internal class UnitOfWork(
    LitraterDbContext context,
    IDateTimeProvider dateTimeProvider,
    IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        StampEntityDates();
        return await context.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = CollectAndClearDomainEvents();
        while (domainEvents.Count > 0)
        {
            foreach (var domainEvent in domainEvents)
            {
                await domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
            }

            domainEvents = CollectAndClearDomainEvents();
        }
    }

    private List<IDomainEvent> CollectAndClearDomainEvents()
    {
        var aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        return domainEvents;
    }

    private void StampEntityDates()
    {
        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedDate(dateTimeProvider.UtcNow);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModifiedDate(dateTimeProvider.UtcNow);
                    break;
            }
        }
    }
}