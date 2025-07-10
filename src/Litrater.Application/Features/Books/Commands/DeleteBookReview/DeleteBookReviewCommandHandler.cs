using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

public sealed class DeleteBookReviewCommandHandler(
    IBookReviewRepository bookReviewRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBookReviewCommand>
{
    public async Task<Result> Handle(DeleteBookReviewCommand command, CancellationToken cancellationToken)
    {
        var bookReview = await bookReviewRepository.GetByIdAsync(command.Id, cancellationToken);
        if (bookReview is null)
        {
            return Result.NotFound();
        }

        // Only the owner or admins can delete their review
        if (bookReview.UserId != command.UserId && !command.IsAdmin)
        {
            return Result.Forbidden();
        }

        bookReviewRepository.Delete(bookReview);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}