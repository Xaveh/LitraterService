using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.UpdateBookReview;

internal sealed class UpdateBookReviewCommandHandler(
    IBookCommandRepository bookCommandRepository,
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

        // Only the owner or admins can update their review
        if (review.UserId != command.UserId && !command.IsAdmin)
        {
            return Result.Forbidden();
        }

        book.UpdateReview(command.Id, command.Content, new Rating(command.Rating));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return review.ToDto();
    }
}