using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

internal sealed class DeleteBookReviewCommandHandler(
    IBookCommandRepository bookCommandRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBookReviewCommand>
{
    public async Task<Result> Handle(DeleteBookReviewCommand command, CancellationToken cancellationToken)
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

        book.RemoveReview(command.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}