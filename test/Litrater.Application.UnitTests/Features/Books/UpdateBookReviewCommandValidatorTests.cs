using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Commands.UpdateBookReview;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookReviewCommandValidatorTests
{
    private readonly UpdateBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 4, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.Empty, "Updated content", 4, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Book review ID must not be empty.");
    }

    [Fact]
    public void Validate_WhenContentIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "", 4, Guid.NewGuid());

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
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), longContent, 4, Guid.NewGuid());

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
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 0, Guid.NewGuid());

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
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 6, Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage("Rating must be between 1 and 5.");
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 4, Guid.Empty);

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
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Admin updated content", 3, Guid.NewGuid(), true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}