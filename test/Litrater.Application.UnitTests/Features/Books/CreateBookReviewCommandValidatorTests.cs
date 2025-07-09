using Litrater.Application.Features.Books.Commands.CreateBookReview;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class CreateBookReviewCommandValidatorTests
{
    private readonly CreateBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldReturnTrue()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenContentIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new CreateBookReviewCommand("", 5, Guid.NewGuid(), Guid.NewGuid());

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
        var command = new CreateBookReviewCommand(longContent, 5, Guid.NewGuid(), Guid.NewGuid());

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
        var command = new CreateBookReviewCommand("Great book!", 0, Guid.NewGuid(), Guid.NewGuid());

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
        var command = new CreateBookReviewCommand("Great book!", 6, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Rating));
    }

    [Fact]
    public void Validate_WhenBookIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.Empty, Guid.NewGuid());

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.BookId));
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.NewGuid(), Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.UserId));
    }
}