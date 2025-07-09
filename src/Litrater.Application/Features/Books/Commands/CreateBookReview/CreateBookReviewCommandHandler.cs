using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.CreateBookReview;

internal sealed class CreateBookReviewCommandHandler(
    IBookReviewRepository bookReviewRepository,
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBookReviewCommand, BookReviewDto>
{
    public async Task<Result<BookReviewDto>> Handle(CreateBookReviewCommand command, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(command.BookId, cancellationToken);
        if (book is null)
        {
            return Result<BookReviewDto>.NotFound();
        }

        if (await bookReviewRepository.ExistsByUserAndBookAsync(command.UserId, command.BookId, cancellationToken))
        {
            return Result<BookReviewDto>.Conflict();
        }

        var bookReview = new BookReview(
            id: Guid.NewGuid(),
            content: command.Content,
            rating: command.Rating,
            bookId: command.BookId,
            userId: command.UserId);

        await bookReviewRepository.AddAsync(bookReview, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return bookReview.ToDto();
    }
}