using Litrater.Application.Abstractions.Data;
using Litrater.Application.Abstractions.DomainEvents;
using Litrater.Domain.Books.Events;

namespace Litrater.Application.Features.Books.EventHandlers;

internal sealed class BookReviewRatingChangedDomainEventHandler(
    IBookCommandRepository bookCommandRepository) : IDomainEventHandler<BookReviewRatingChangedDomainEvent>
{
    public async Task Handle(BookReviewRatingChangedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var ratings = await bookCommandRepository
            .GetReviewsAsync(domainEvent.BookId, cancellationToken);

        double? newAverage = (domainEvent.OldRating, domainEvent.NewRating) switch
        {
            (null, { } newRating) =>
                ratings.Count == 0
                    ? (double?)newRating
                    : (ratings.Sum(r => (double)r.Value) + newRating) / (ratings.Count + 1),
            ({ } oldRating, null) =>
                ratings.Count <= 1
                    ? null
                    : (ratings.Sum(r => (double)r.Value) - oldRating) / (ratings.Count - 1),
            ({ } oldRating, { } newRating) =>
                ratings.Count == 0
                    ? (double?)newRating
                    : (ratings.Sum(r => (double)r.Value) - oldRating + newRating) / ratings.Count,
            _ => ratings.Count == 0 ? null : ratings.Average(r => r.Value)
        };

        await bookCommandRepository.SetAverageRatingAsync(domainEvent.BookId, newAverage, cancellationToken);
    }
}