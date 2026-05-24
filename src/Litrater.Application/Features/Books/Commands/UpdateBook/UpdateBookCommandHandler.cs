using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Commands.UpdateBook;

internal sealed class UpdateBookCommandHandler(
    IBookCommandRepository bookCommandRepository,
    IAuthorCommandRepository authorCommandRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateBookCommand, BookDto>
{
    public async Task<Result<BookDto>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await bookCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result.NotFound();
        }

        var authors = await authorCommandRepository.GetAuthorsByIdsAsync(command.AuthorIds, cancellationToken);
        if (authors.Count != command.AuthorIds.Count())
        {
            return Result<BookDto>.Invalid(new ValidationError(nameof(command.AuthorIds),
                $"Some author IDs are invalid or missing. Requested: {command.AuthorIds.Count()}, Found: {authors.Count}"));
        }

        book.Update(command.Title, new Isbn(command.Isbn), authors);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return book.ToDto();
    }
}