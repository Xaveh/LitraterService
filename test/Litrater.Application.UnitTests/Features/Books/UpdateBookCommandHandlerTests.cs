using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.UpdateBook;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateBookCommandHandler _handler;

    public UpdateBookCommandHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookExistsAndAuthorsExist_ShouldUpdateBookSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new UpdateBookCommand(bookId, "Updated Book", "9876543210123", authorIds);
        var existingAuthors = new List<Author> { new(Guid.NewGuid(), "Old", "Author") };
        var newAuthors = new List<Author> { new(authorIds[0], "John", "Doe"), new(authorIds[1], "Jane", "Smith") };
        var book = new Book(bookId, "Old Book", "1234567890123", existingAuthors);

        _bookRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _authorRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAuthors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(bookId);
        result.Value.Title.ShouldBe(command.Title);
        result.Value.Isbn.ShouldBe(command.Isbn);
        result.Value.AuthorIds.ShouldBe(authorIds);

        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var authorIds = new List<Guid> { Guid.NewGuid() };
        var command = new UpdateBookCommand(bookId, "Updated Book", "9876543210123", authorIds);

        _bookRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSomeAuthorIdsAreInvalid_ShouldReturnInvalidResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new UpdateBookCommand(bookId, "Updated Book", "9876543210123", authorIds);
        var existingAuthors = new List<Author> { new(Guid.NewGuid(), "Old", "Author") };
        var book = new Book(bookId, "Old Book", "1234567890123", existingAuthors);
        var foundAuthors = new List<Author> { new(authorIds[0], "John", "Doe") }; // Only one author found

        _bookRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _authorRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(foundAuthors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(command.AuthorIds));

        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
} 