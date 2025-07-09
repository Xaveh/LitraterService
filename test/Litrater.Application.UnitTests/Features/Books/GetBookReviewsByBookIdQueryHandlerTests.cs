using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByBookId;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBookReviewsByBookIdQueryHandlerTests
{
    private readonly Mock<IBookReviewRepository> _mockBookReviewRepository;
    private readonly GetBookReviewsByBookIdQueryHandler _handler;

    public GetBookReviewsByBookIdQueryHandlerTests()
    {
        _mockBookReviewRepository = new Mock<IBookReviewRepository>();
        _handler = new GetBookReviewsByBookIdQueryHandler(_mockBookReviewRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPagedResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetBookReviewsByBookIdQuery(bookId, 1, 10);
        var reviews = new List<BookReview>
        {
            new(Guid.NewGuid(), "Great book!", 5, bookId, userId),
            new(Guid.NewGuid(), "Good read", 4, bookId, userId)
        };

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByBookIdAsync(bookId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 2));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.Count().ShouldBe(2);
        result.Value.PagedInfo.PageNumber.ShouldBe(1);
        result.Value.PagedInfo.PageSize.ShouldBe(10);
        result.Value.PagedInfo.TotalRecords.ShouldBe(2);
        result.Value.PagedInfo.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_WithEmptyResult_ShouldReturnEmptyPagedResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookReviewsByBookIdQuery(bookId, 1, 10);
        var reviews = new List<BookReview>();

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByBookIdAsync(bookId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 0));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.Count().ShouldBe(0);
        result.Value.PagedInfo.PageNumber.ShouldBe(1);
        result.Value.PagedInfo.PageSize.ShouldBe(10);
        result.Value.PagedInfo.TotalRecords.ShouldBe(0);
        result.Value.PagedInfo.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_WithMultiplePages_ShouldReturnCorrectPaginationInfo()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetBookReviewsByBookIdQuery(bookId, 2, 5);
        var reviews = new List<BookReview>
        {
            new(Guid.NewGuid(), "Review 1", 5, bookId, userId),
            new(Guid.NewGuid(), "Review 2", 4, bookId, userId)
        };

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByBookIdAsync(bookId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 12)); // Total of 12 items

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.Count().ShouldBe(2);
        result.Value.PagedInfo.PageNumber.ShouldBe(2);
        result.Value.PagedInfo.PageSize.ShouldBe(5);
        result.Value.PagedInfo.TotalRecords.ShouldBe(12);
        result.Value.PagedInfo.TotalPages.ShouldBe(3); // 12 items / 5 per page = 3 pages
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookReviewsByBookIdQuery(bookId, 2, 15);
        var reviews = new List<BookReview>();

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByBookIdAsync(bookId, 2, 15, It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 0));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockBookReviewRepository.Verify(
            x => x.GetBookReviewsByBookIdAsync(bookId, query.Page, query.PageSize, It.IsAny<CancellationToken>()), Times.Once);
    }
}