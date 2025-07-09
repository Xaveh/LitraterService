using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Queries.GetBooks;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBooksQueryHandlerTests
{
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly GetBooksQueryHandler _handler;

    public GetBooksQueryHandlerTests()
    {
        _mockBookRepository = new Mock<IBookRepository>();
        _handler = new GetBooksQueryHandler(_mockBookRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPagedResult()
    {
        // Arrange
        var query = new GetBooksQuery(1, 10);
        var author = new Author(Guid.NewGuid(), "John", "Doe");
        var books = new List<Book>
        {
            new(Guid.NewGuid(), "Book 1", "123", [author]),
            new(Guid.NewGuid(), "Book 2", "456", [author])
        };

        _mockBookRepository
            .Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((books, 2));

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
        var query = new GetBooksQuery(1, 10);
        var books = new List<Book>();

        _mockBookRepository
            .Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((books, 0));

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
        var query = new GetBooksQuery(2, 5);
        var author = new Author(Guid.NewGuid(), "Jane", "Smith");
        var books = new List<Book>
        {
            new(Guid.NewGuid(), "Book 1", "123", [author]),
            new(Guid.NewGuid(), "Book 2", "456", [author])
        };

        _mockBookRepository
            .Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((books, 12)); // Total of 12 items

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
}