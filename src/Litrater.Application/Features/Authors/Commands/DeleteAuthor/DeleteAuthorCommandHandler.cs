using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Authors.Commands.DeleteAuthor;

public sealed class DeleteAuthorCommandHandler(IAuthorRepository authorRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteAuthorCommand>
{
    public async Task<Result> Handle(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(command.Id, cancellationToken);
        if (author is null)
        {
            return Result.NotFound($"Author with ID {command.Id} not found.");
        }

        authorRepository.Delete(author);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}