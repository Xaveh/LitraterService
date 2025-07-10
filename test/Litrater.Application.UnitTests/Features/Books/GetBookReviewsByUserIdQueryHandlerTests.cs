using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBookReviewsByUserIdQueryHandlerTests
{
    private readonly Mock<IBookReviewRepository> _mockBookReviewRepository;
    private readonly GetBookReviewsByUserIdQueryHandler _handler;

    public GetBookReviewsByUserIdQueryHandlerTests()
    {
        _mockBookReviewRepository = new Mock<IBookReviewRepository>();
        _handler = new GetBookReviewsByUserIdQueryHandler(_mockBookReviewRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var query = new GetBookReviewsByUserIdQuery(userId, 1, 10);
        var reviews = new List<BookReview>
        {
            new(Guid.NewGuid(), "Great book!", 5, bookId, userId),
            new(Guid.NewGuid(), "Good read", 4, bookId, userId)
        };

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
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
        var userId = Guid.NewGuid();
        var query = new GetBookReviewsByUserIdQuery(userId, 1, 10);
        var reviews = new List<BookReview>();

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
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
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var query = new GetBookReviewsByUserIdQuery(userId, 2, 5);
        var reviews = new List<BookReview>
        {
            new(Guid.NewGuid(), "Review 1", 5, bookId, userId),
            new(Guid.NewGuid(), "Review 2", 4, bookId, userId)
        };

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
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
        var userId = Guid.NewGuid();
        var query = new GetBookReviewsByUserIdQuery(userId, 2, 15);
        var reviews = new List<BookReview>();

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByUserIdAsync(userId, 2, 15, It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 0));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockBookReviewRepository.Verify(
            x => x.GetBookReviewsByUserIdAsync(userId, query.Page, query.PageSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBookReviewDtosWithCorrectData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var query = new GetBookReviewsByUserIdQuery(userId, 1, 10);
        var reviews = new List<BookReview>
        {
            new(reviewId, "Excellent book!", 5, bookId, userId)
        };

        _mockBookReviewRepository
            .Setup(x => x.GetBookReviewsByUserIdAsync(userId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((reviews, 1));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var reviewDto = result.Value.Value.First();
        reviewDto.Id.ShouldBe(reviewId);
        reviewDto.Content.ShouldBe("Excellent book!");
        reviewDto.Rating.ShouldBe(5);
        reviewDto.BookId.ShouldBe(bookId);
        reviewDto.UserId.ShouldBe(userId);
    }
} 