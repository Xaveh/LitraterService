using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Microsoft.Extensions.Logging;

namespace Litrater.Application.Behaviors;

public sealed class CommandHandlerLoggingDecorator<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    ILogger<CommandHandlerLoggingDecorator<TCommand>> logger)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        logger.LogInformation("Handling command: {CommandType}", commandName);

        var result = await innerHandler.Handle(command, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Command {CommandType} processed successfully.", commandName);
        }
        else
        {
            var allErrors = result.Errors.Concat(result.ValidationErrors.Select(x => x.ErrorMessage));
            logger.LogWarning("Command {CommandType} failed. Status: {Status}, Errors: {Errors}", commandName, result.Status,
                string.Join(", ", allErrors));
        }

        return result;
    }
}

public sealed class CommandHandlerLoggingDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ILogger<CommandHandlerLoggingDecorator<TCommand, TResponse>> logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        logger.LogInformation("Handling command: {CommandType}", commandName);

        var result = await innerHandler.Handle(command, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Command {CommandType} processed successfully.", commandName);
        }
        else
        {
            var allErrors = result.Errors.Concat(result.ValidationErrors.Select(x => x.ErrorMessage));
            logger.LogWarning("Command {CommandType} failed. Status: {Status}, Errors: {Errors}", commandName, result.Status,
                string.Join(", ", allErrors));
        }

        return result;
    }
}