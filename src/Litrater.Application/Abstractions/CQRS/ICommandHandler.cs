using Ardalis.Result;

namespace Litrater.Application.Abstractions.CQRS;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}
