using FluentValidation.TestHelper;
using Litrater.Application.Features.Authentication.Commands.Register;

namespace Litrater.Application.UnitTests.Features.Authentication;

public sealed class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenEmailIsInvalid_ShouldHaveValidationError(string? email)
    {
        // Arrange
        var command = new RegisterCommand(email!, "password123", "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    public void Validate_WhenEmailFormatIsInvalid_ShouldHaveValidationError(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "password123", "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError(string? password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password!, "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("pass")]
    public void Validate_WhenPasswordIsTooShort_ShouldHaveValidationError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password, "John", "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenFirstNameIsEmpty_ShouldHaveValidationError(string? firstName)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", firstName!, "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Validate_WhenFirstNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longFirstName = new string('a', 101);
        var command = new RegisterCommand("test@example.com", "password123", longFirstName, "Doe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenLastNameIsEmpty_ShouldHaveValidationError(string? lastName)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "John", lastName!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Validate_WhenLastNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longLastName = new string('a', 101);
        var command = new RegisterCommand("test@example.com", "password123", "John", longLastName);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name cannot exceed 100 characters");
    }
}
