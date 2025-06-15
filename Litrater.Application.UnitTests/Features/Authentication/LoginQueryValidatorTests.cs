using FluentValidation.TestHelper;
using Litrater.Application.Features.Authentication.Queries.Login;

namespace Litrater.Application.UnitTests.Features.Authentication;

public sealed class LoginQueryValidatorTests
{
    private readonly LoginQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenEmailAndPasswordAreValid_ShouldBeValid()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "password123");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenEmailIsNullOrEmpty_ShouldBeInvalid(string? email)
    {
        // Arrange
        var query = new LoginQuery(email!, "password123");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    public void Validate_WhenEmailIsInvalid_ShouldBeInvalid(string email)
    {
        // Arrange
        var query = new LoginQuery(email, "password123");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must be a valid email address");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WhenPasswordIsNullOrEmpty_ShouldBeInvalid(string? password)
    {
        // Arrange
        var query = new LoginQuery("test@example.com", password!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }
}