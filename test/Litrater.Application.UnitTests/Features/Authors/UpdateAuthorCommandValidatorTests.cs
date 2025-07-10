using FluentValidation.TestHelper;
using Litrater.Application.Features.Authors.Commands.UpdateAuthor;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class UpdateAuthorCommandValidatorTests
{
    private readonly UpdateAuthorCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.Empty,
            FirstName: "John",
            LastName: "Doe",
            BookIds: [Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenFirstNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "",
            LastName: "Doe",
            BookIds: [Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: "",
            BookIds: [Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenFirstNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: new string('a', 101),
            LastName: "Doe",
            BookIds: [Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenLastNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: new string('a', 101),
            BookIds: [Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenBookIdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            BookIds: []
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BookIds);
    }

    [Fact]
    public void Validate_WhenBookIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            BookIds: [Guid.Empty]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("BookIds[0]");
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            BookIds: [Guid.NewGuid(), Guid.NewGuid()]
        );

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}