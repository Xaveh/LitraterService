using Litrater.Domain.Common;

namespace Litrater.Domain.Books.Events;

public sealed record BookReviewRatingChangedDomainEvent(Guid BookId, int? OldRating, int? NewRating) : IDomainEvent;
