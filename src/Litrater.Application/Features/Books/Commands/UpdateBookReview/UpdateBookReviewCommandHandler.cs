using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.UpdateBookReview;

public sealed class UpdateBookReviewCommandHandler(
    IBookReviewCommandRepository bookReviewCommandRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBookReviewCommand, BookReviewDto>
{
    public async Task<Result<BookReviewDto>> Handle(UpdateBookReviewCommand command, CancellationToken cancellationToken)
    {
        var bookReview = await bookReviewCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (bookReview is null)
        {
            return Result.NotFound();
        }

        // Only the owner or admins can update their review
        if (bookReview.UserId != command.UserId && !command.IsAdmin)
        {
            return Result.Forbidden();
        }

        bookReview.Update(command.Content, new Rating(command.Rating));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return bookReview.ToDto();
    }
}