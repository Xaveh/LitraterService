using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Commands.DeleteBookReview;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookReviewCommandValidatorTests
{
    private readonly DeleteBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Book review ID must not be empty.");
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID must not be empty.");
    }

    [Fact]
    public void Validate_WhenIsAdminIsTrue_ShouldStillBeValid()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenBothIdAndUserIdAreEmpty_ShouldHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.Empty, Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Book review ID must not be empty.");
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID must not be empty.");
    }
}