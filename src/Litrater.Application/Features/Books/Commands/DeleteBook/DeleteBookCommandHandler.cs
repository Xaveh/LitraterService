using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;

namespace Litrater.Application.Features.Books.Commands.DeleteBook;

public sealed class DeleteBookCommandHandler(IBookRepository bookRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result.NotFound($"Book with ID {command.Id} not found.");
        }

        bookRepository.Delete(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}