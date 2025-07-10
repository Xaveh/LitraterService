using FluentValidation.TestHelper;
using Litrater.Application.Features.Authors.Commands.CreateAuthor;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class CreateAuthorCommandValidatorTests
{
    private readonly CreateAuthorCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllPropertiesAreValid_ShouldBeValid()
    {
        // Arrange
        var command = new CreateAuthorCommand("John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenFirstNameIsNullOrEmpty_ShouldBeInvalid(string? firstName)
    {
        // Arrange
        var command = new CreateAuthorCommand(firstName!, "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenFirstNameExceedsMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var longFirstName = new string('a', 101);
        var command = new CreateAuthorCommand(longFirstName, "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenLastNameIsNullOrEmpty_ShouldBeInvalid(string? lastName)
    {
        // Arrange
        var command = new CreateAuthorCommand("John", lastName!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WhenLastNameExceedsMaxLength_ShouldBeInvalid()
    {
        // Arrange
        var longLastName = new string('a', 101);
        var command = new CreateAuthorCommand("John", longLastName);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}