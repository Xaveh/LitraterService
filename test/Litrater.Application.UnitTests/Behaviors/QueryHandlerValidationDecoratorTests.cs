using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Behaviors;
using Litrater.Application.UnitTests.Common;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Behaviors;

public sealed class QueryHandlerValidationDecoratorTests
{
    private readonly Mock<IQueryHandler<TestQuery, string>> _innerHandlerMock;
    private readonly Mock<IValidator<TestQuery>> _validatorMock;
    private readonly QueryHandlerValidationDecorator<TestQuery, string> _decorator;

    public QueryHandlerValidationDecoratorTests()
    {
        _innerHandlerMock = new Mock<IQueryHandler<TestQuery, string>>();
        _validatorMock = new Mock<IValidator<TestQuery>>();
        _decorator = new QueryHandlerValidationDecorator<TestQuery, string>(_innerHandlerMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallInnerHandler()
    {
        // Arrange
        var query = new TestQuery();
        var validationResult = new ValidationResult();
        var expectedResult = Result<string>.Success("test result");

        _validatorMock
            .Setup(x => x.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _innerHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _decorator.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        _innerHandlerMock.Verify(x => x.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnInvalidResult()
    {
        // Arrange
        var query = new TestQuery();
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Error message 1"),
            new("Property2", "Error message 2")
        };
        var validationResult = new ValidationResult(validationFailures);

        _validatorMock
            .Setup(x => x.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _decorator.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.Count().ShouldBe(2);
        _innerHandlerMock.Verify(x => x.Handle(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidatorIsNull_ShouldCallInnerHandlerDirectly()
    {
        // Arrange
        var query = new TestQuery();
        var expectedResult = Result<string>.Success("test result");
        var decoratorWithoutValidator = new QueryHandlerValidationDecorator<TestQuery, string>(_innerHandlerMock.Object, null);

        _innerHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await decoratorWithoutValidator.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        _innerHandlerMock.Verify(x => x.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
    }
}
