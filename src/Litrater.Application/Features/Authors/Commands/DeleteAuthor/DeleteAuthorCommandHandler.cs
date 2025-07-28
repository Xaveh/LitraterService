using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Authors.Commands.DeleteAuthor;

internal sealed class DeleteAuthorCommandHandler(IAuthorCommandRepository authorCommandRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteAuthorCommand>
{
    public async Task<Result> Handle(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = await authorCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (author is null)
        {
            return Result.NotFound($"Author with ID {command.Id} not found.");
        }

        authorCommandRepository.Delete(author);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}