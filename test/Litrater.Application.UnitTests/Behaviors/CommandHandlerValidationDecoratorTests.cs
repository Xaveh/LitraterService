using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Behaviors;
using Litrater.Application.UnitTests.Common;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Behaviors;

public sealed class CommandHandlerValidationDecoratorTests
{
    private readonly CommandHandlerValidationDecorator<TestCommand> _decorator;
    private readonly Mock<ICommandHandler<TestCommand>> _innerHandlerMock;
    private readonly Mock<IValidator<TestCommand>> _validatorMock;

    public CommandHandlerValidationDecoratorTests()
    {
        _innerHandlerMock = new Mock<ICommandHandler<TestCommand>>();
        _validatorMock = new Mock<IValidator<TestCommand>>();
        _decorator = new CommandHandlerValidationDecorator<TestCommand>(_innerHandlerMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallInnerHandler()
    {
        // Arrange
        var command = new TestCommand();
        var validationResult = new ValidationResult();
        var expectedResult = Result.Success();

        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _decorator.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnInvalidResult()
    {
        // Arrange
        var command = new TestCommand();
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Error message 1"),
            new("Property2", "Error message 2")
        };
        var validationResult = new ValidationResult(validationFailures);

        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _decorator.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.Count().ShouldBe(2);
        _innerHandlerMock.Verify(x => x.Handle(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidatorIsNull_ShouldCallInnerHandlerDirectly()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResult = Result.Success();
        var decoratorWithoutValidator = new CommandHandlerValidationDecorator<TestCommand>(_innerHandlerMock.Object, null);

        _innerHandlerMock
            .Setup(x => x.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await decoratorWithoutValidator.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);
        _innerHandlerMock.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once);
    }
}