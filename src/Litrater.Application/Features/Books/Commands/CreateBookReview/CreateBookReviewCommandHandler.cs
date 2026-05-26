using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.CreateBookReview;

internal sealed class CreateBookReviewCommandHandler(
    IBookCommandRepository bookCommandRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBookReviewCommand, BookReviewDto>
{
    public async Task<Result<BookReviewDto>> Handle(CreateBookReviewCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByKeycloakUserIdAsync(command.KeycloakUserId, cancellationToken);
        if (user is null)
        {
            return Result<BookReviewDto>.Unauthorized();
        }

        var book = await bookCommandRepository.GetByIdAsync(command.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookReviewDto>.NotFound();
        }

        var review = book.AddReview(
            id: Guid.NewGuid(),
            content: command.Content,
            rating: new Rating(command.Rating),
            userId: user.Id);

        if (review is null)
        {
            return Result<BookReviewDto>.Conflict();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return review.ToDto();
    }
}