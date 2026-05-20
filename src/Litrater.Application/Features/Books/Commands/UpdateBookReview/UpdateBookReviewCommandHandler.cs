using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.UpdateBookReview;

internal sealed class UpdateBookReviewCommandHandler(
    IBookCommandRepository bookCommandRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBookReviewCommand, BookReviewDto>
{
    public async Task<Result<BookReviewDto>> Handle(UpdateBookReviewCommand command, CancellationToken cancellationToken)
    {
        var book = await bookCommandRepository.GetByReviewIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result.NotFound();
        }

        var review = book.FindReview(command.Id)!;

        if (!command.IsAdmin)
        {
            var user = await userRepository.GetByKeycloakUserIdAsync(command.KeycloakUserId, cancellationToken);
            if (user is null)
            {
                return Result.Unauthorized();
            }

            if (review.UserId != user.Id)
            {
                return Result.Forbidden();
            }
        }

        book.UpdateReview(command.Id, command.Content, new Rating(command.Rating));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return review.ToDto();
    }
}