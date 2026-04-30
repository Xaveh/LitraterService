using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Domain.Authors;

namespace Litrater.Application.Features.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandHandler(
    IAuthorCommandRepository authorCommandRepository,
    IBookCommandRepository bookCommandRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateAuthorCommand, AuthorDto>
{
    public async Task<Result<AuthorDto>> Handle(UpdateAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = await authorCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (author is null)
        {
            return Result.NotFound();
        }

        var books = await bookCommandRepository.GetBooksByIdsAsync(command.BookIds, cancellationToken);
        if (books.Count != command.BookIds.Count())
        {
            return Result<AuthorDto>.Invalid(new ValidationError(nameof(command.BookIds),
                $"Some book IDs are invalid or missing. Requested: {command.BookIds.Count()}, Found: {books.Count}"));
        }

        author.Update(new PersonName(command.FirstName, command.LastName), books);
        authorCommandRepository.Update(author);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return author.ToDto();
    }
}