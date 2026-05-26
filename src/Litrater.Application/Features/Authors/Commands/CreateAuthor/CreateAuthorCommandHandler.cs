using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Domain.Authors;

namespace Litrater.Application.Features.Authors.Commands.CreateAuthor;

internal sealed class CreateAuthorCommandHandler(
    IAuthorCommandRepository authorCommandRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateAuthorCommand, AuthorDto>
{
    public async Task<Result<AuthorDto>> Handle(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        if (await authorCommandRepository.ExistsByNameAsync(command.FirstName, command.LastName, cancellationToken))
        {
            return Result<AuthorDto>.Conflict();
        }

        var author = new Author(
            id: Guid.NewGuid(),
            name: new PersonName(command.FirstName, command.LastName));

        await authorCommandRepository.AddAsync(author, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return author.ToDto();
    }
}