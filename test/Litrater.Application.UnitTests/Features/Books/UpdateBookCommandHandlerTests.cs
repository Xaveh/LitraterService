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
    private readonly Mock<IAuthorCommandRepository> _authorCommandRepositoryMock;
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly UpdateBookCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateBookCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _authorCommandRepositoryMock = new Mock<IAuthorCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateBookCommandHandler(_bookCommandRepositoryMock.Object, _authorCommandRepositoryMock.Object, _unitOfWorkMock.Object);
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

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _authorCommandRepositoryMock
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

        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorCommandRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var authorIds = new List<Guid> { Guid.NewGuid() };
        var command = new UpdateBookCommand(bookId, "Updated Book", "9876543210123", authorIds);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorCommandRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
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

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _authorCommandRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(foundAuthors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(command.AuthorIds));

        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        _authorCommandRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(authorIds, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}