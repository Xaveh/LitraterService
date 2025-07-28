using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBooks;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBooksQueryHandlerTests
{
    private readonly GetBooksQueryHandler _handler;
    private readonly Mock<IBookQueryRepository> _bookQueryRepositoryMock;

    public GetBooksQueryHandlerTests()
    {
        _bookQueryRepositoryMock = new Mock<IBookQueryRepository>();
        _handler = new GetBooksQueryHandler(_bookQueryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnPagedResult()
    {
        // Arrange
        var query = new GetBooksQuery();
        var authorId = Guid.NewGuid();
        var bookDtos = new List<BookDto>
        {
            new(Guid.NewGuid(), "Book 1", "123", [authorId], []),
            new(Guid.NewGuid(), "Book 2", "456", [authorId], [])
        };

        _bookQueryRepositoryMock
            .Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((bookDtos, 2));

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
        var query = new GetBooksQuery();
        var bookDtos = new List<BookDto>();

        _bookQueryRepositoryMock
            .Setup(x => x.GetBooksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((bookDtos, 0));

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
        var authorId = Guid.NewGuid();
        var books = new List<BookDto>
        {
            new(Guid.NewGuid(), "Book 1", "123", [authorId], []),
            new(Guid.NewGuid(), "Book 2", "456", [authorId], [])
        };

        _bookQueryRepositoryMock
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