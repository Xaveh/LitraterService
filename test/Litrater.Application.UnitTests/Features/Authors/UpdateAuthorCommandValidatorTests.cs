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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Author ID is required");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name cannot exceed 100 characters");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name cannot exceed 100 characters");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookIds)
            .WithErrorMessage("Book IDs must not be empty.");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("BookIds[0]")
            .WithErrorMessage("Book IDs must not be empty.");
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}