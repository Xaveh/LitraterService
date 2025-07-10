using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByBookId;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBookReviewsByBookIdQueryValidatorTests
{
    private readonly GetBookReviewsByBookIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.NewGuid(), 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyBookId_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.Empty, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BookId)
            .WithErrorMessage("BookId is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidPage_ShouldHaveValidationError(int page)
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.NewGuid(), page, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidPageSize_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.NewGuid(), 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be greater than 0");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    [InlineData(1000)]
    public void Validate_WithPageSizeExceedingLimit_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.NewGuid(), 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must not exceed 100");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_WithValidPageSize_ShouldNotHaveValidationErrors(int pageSize)
    {
        // Arrange
        var query = new GetBookReviewsByBookIdQuery(Guid.NewGuid(), 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 