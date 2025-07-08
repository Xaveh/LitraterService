using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Commands.UpdateBook;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookCommandValidatorTests
{
    private readonly UpdateBookCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllPropertiesAreValid_ShouldBeValid()
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), "Updated Book", "1234567890123", [Guid.NewGuid(), Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.Empty, "Updated Book", "1234567890123", [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenTitleIsNullOrEmpty_ShouldBeInvalid(string? title)
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), title!, "1234567890123", [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_WhenTitleExceedsMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new UpdateBookCommand(Guid.NewGuid(), longTitle, "1234567890123", [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenIsbnIsNullOrEmpty_ShouldBeInvalid(string? isbn)
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), "Updated Book", isbn!, [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Isbn);
    }

    [Theory]
    [InlineData("123456789012")]
    [InlineData("12345678901234")]
    [InlineData("123456789012a")]
    [InlineData("abcdefghijklm")]
    public void Validate_WhenIsbnIsInvalid_ShouldBeInvalid(string isbn)
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), "Updated Book", isbn, [Guid.NewGuid()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Isbn)
            .WithErrorMessage("ISBN must be exactly 13 digits.");
    }

    [Fact]
    public void Validate_WhenAuthorIdsIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), "Updated Book", "1234567890123", []);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorIds);
    }

    [Fact]
    public void Validate_WhenAuthorIdsContainsEmptyGuid_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateBookCommand(Guid.NewGuid(), "Updated Book", "1234567890123", [Guid.NewGuid(), Guid.Empty]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorIds)
            .WithErrorMessage("Author IDs must not be empty.");
    }
} 