using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Behaviors;
using Litrater.Application.UnitTests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Behaviors;

public sealed class QueryHandlerLoggingDecoratorTests
{
    private readonly Mock<IQueryHandler<TestQuery, string>> _innerHandlerMock;
    private readonly Mock<ILogger<QueryHandlerLoggingDecorator<TestQuery, string>>> _loggerMock;
    private readonly QueryHandlerLoggingDecorator<TestQuery, string> _decorator;

    public QueryHandlerLoggingDecoratorTests()
    {
        _innerHandlerMock = new Mock<IQueryHandler<TestQuery, string>>();
        _loggerMock = new Mock<ILogger<QueryHandlerLoggingDecorator<TestQuery, string>>>();
        _decorator = new QueryHandlerLoggingDecorator<TestQuery, string>(_innerHandlerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenQuerySucceeds_ShouldLogStartAndSuccess()
    {
        // Arrange
        var query = new TestQuery();
        var successResult = Result<string>.Success("test result");

        _innerHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _decorator.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe(successResult);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling query: TestQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Query TestQuery processed successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenQueryFails_ShouldLogStartAndFailure()
    {
        // Arrange
        var query = new TestQuery();
        var failureResult = Result<string>.Error("Test error");

        _innerHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _decorator.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe(failureResult);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling query: TestQuery")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Query TestQuery failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallInnerHandler()
    {
        // Arrange
        var query = new TestQuery();
        var result = Result<string>.Success("test");

        _innerHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _decorator.Handle(query, CancellationToken.None);

        // Assert
        _innerHandlerMock.Verify(x => x.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}
