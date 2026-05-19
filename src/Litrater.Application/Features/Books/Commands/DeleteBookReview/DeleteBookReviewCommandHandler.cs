using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

internal sealed class DeleteBookReviewCommandHandler(
    IBookCommandRepository bookCommandRepository,
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

        // Only the owner or admins can delete their review
        if (review.UserId != command.UserId && !command.IsAdmin)
        {
            return Result.Forbidden();
        }

        book.RemoveReview(command.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}