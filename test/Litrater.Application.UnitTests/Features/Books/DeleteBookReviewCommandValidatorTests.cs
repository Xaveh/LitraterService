using Litrater.Application.Features.Books.Commands.DeleteBookReview;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookReviewCommandValidatorTests
{
    private readonly DeleteBookReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldReturnTrue()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.NewGuid(), false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.Empty, Guid.NewGuid(), false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Id));
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.Empty, false);

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
        var command = new DeleteBookReviewCommand(Guid.NewGuid(), Guid.NewGuid(), true);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_WhenBothIdAndUserIdAreEmpty_ShouldReturnFalseWithMultipleErrors()
    {
        // Arrange
        var command = new DeleteBookReviewCommand(Guid.Empty, Guid.Empty, false);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(2);
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.Id));
        result.Errors.ShouldContain(e => e.PropertyName == nameof(command.UserId));
    }
}