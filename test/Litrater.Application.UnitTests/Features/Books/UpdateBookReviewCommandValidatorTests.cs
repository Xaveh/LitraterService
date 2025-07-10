using Litrater.Application.Features.Books.Commands.UpdateBookReview;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookReviewCommandValidatorTests
{
    private readonly UpdateBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldReturnTrue()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 4, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.Empty, "Updated content", 4, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Id));
    }

    [Fact]
    public void Validate_WhenContentIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "", 4, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Content));
    }

    [Fact]
    public void Validate_WhenContentIsTooLong_ShouldReturnFalse()
    {
        // Arrange
        var longContent = new string('a', 1001);
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), longContent, 4, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Content));
    }

    [Fact]
    public void Validate_WhenRatingIsZero_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 0, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Rating));
    }

    [Fact]
    public void Validate_WhenRatingIsGreaterThanFive_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 6, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Rating));
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Updated content", 4, Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.UserId));
    }

    [Fact]
    public void Validate_WhenIsAdminIsTrue_ShouldStillBeValid()
    {
        // Arrange
        var command = new UpdateBookReviewCommand(Guid.NewGuid(), "Admin updated content", 3, Guid.NewGuid(), true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}