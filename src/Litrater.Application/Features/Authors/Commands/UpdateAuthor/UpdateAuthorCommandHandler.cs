using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;

namespace Litrater.Application.Features.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandHandler(
    IAuthorRepository authorRepository,
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateAuthorCommand, AuthorDto>
{
    public async Task<Result<AuthorDto>> Handle(UpdateAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(command.Id, cancellationToken);
        if (author is null)
        {
            return Result.NotFound();
        }

        var books = await bookRepository.GetBooksByIdsAsync(command.BookIds, cancellationToken);
        if (books.Count != command.BookIds.Count())
        {
            return Result<AuthorDto>.Invalid(new ValidationError(nameof(command.BookIds),
                $"Some book IDs are invalid or missing. Requested: {command.BookIds.Count()}, Found: {books.Count}"));
        }

        author.Update(command.FirstName, command.LastName, books);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return author.ToDto();
    }
}