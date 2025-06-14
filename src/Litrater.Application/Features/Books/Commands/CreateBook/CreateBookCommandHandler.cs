using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.CreateBook;

internal sealed class CreateBookCommandHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBookCommand, BookDto>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        if (await bookRepository.ExistsByIsbnAsync(command.Isbn, cancellationToken))
        {
            return Result<BookDto>.Conflict();
        }

        var authors = await authorRepository.GetAuthorsByIdsAsync(command.AuthorIds, cancellationToken);

        var book = new Book(
            id: Guid.NewGuid(),
            title: command.Title,
            isbn: command.Isbn,
            authors: authors);

        await bookRepository.AddAsync(book, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return book.ToDto();
    }
}