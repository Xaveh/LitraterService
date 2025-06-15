using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Behaviors;
using Litrater.Application.UnitTests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Behaviors;

public sealed class CommandHandlerLoggingDecoratorTests
{
    private readonly Mock<ICommandHandler<TestCommand>> _innerHandlerMock;
    private readonly Mock<ILogger<CommandHandlerLoggingDecorator<TestCommand>>> _loggerMock;
    private readonly CommandHandlerLoggingDecorator<TestCommand> _decorator;

    public CommandHandlerLoggingDecoratorTests()
    {
        _innerHandlerMock = new Mock<ICommandHandler<TestCommand>>();
        _loggerMock = new Mock<ILogger<CommandHandlerLoggingDecorator<TestCommand>>>();
        _decorator = new CommandHandlerLoggingDecorator<TestCommand>(_innerHandlerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCommandSucceeds_ShouldLogStartAndSuccess()
    {
        // Arrange
        var command = new TestCommand();
        var successResult = Result.Success();

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _decorator.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(successResult);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling command: TestCommand")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Command TestCommand processed successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCommandFails_ShouldLogStartAndFailure()
    {
        // Arrange
        var command = new TestCommand();
        var failureResult = Result.Error("Test error");

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _decorator.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(failureResult);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling command: TestCommand")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Command TestCommand failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallInnerHandler()
    {
        // Arrange
        var command = new TestCommand();
        var result = Result.Success();

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _decorator.Handle(command, CancellationToken.None);

        // Assert
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}
