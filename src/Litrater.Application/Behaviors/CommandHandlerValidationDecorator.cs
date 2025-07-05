using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Behaviors;

internal sealed class CommandHandlerValidationDecorator<TCommand>(
    ICommandHandler<TCommand> inner,
    IValidator<TCommand>? validator)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Result.Invalid(result.AsErrors());
            }
        }

        return await inner.Handle(command, cancellationToken);
    }
}

internal sealed class CommandHandlerValidationDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IValidator<TCommand>? validator)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return Result<TResponse>.Invalid(result.AsErrors());
            }
        }

        return await inner.Handle(command, cancellationToken);
    }
}
