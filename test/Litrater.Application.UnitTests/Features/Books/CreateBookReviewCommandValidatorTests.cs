using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Commands.CreateBookReview;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class CreateBookReviewCommandValidatorTests
{
    private readonly CreateBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenContentIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateBookReviewCommand("", 5, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content is required");
    }

    [Fact]
    public void Validate_WhenContentIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longContent = new string('a', 1001);
        var command = new CreateBookReviewCommand(longContent, 5, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("Content cannot exceed 1000 characters");
    }

    [Fact]
    public void Validate_WhenRatingIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 0, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 5.");
    }

    [Fact]
    public void Validate_WhenRatingIsGreaterThanFive_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 6, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 5.");
    }

    [Fact]
    public void Validate_WhenBookIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookId)
            .WithErrorMessage("Book ID must not be empty.");
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.NewGuid(), Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User ID must not be empty.");
    }
}