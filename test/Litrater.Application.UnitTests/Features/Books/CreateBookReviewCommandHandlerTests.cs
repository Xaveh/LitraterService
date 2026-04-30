using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.CreateBookReview;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class CreateBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly Mock<IBookReviewCommandRepository> _bookReviewCommandRepositoryMock;
    private readonly CreateBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateBookReviewCommandHandlerTests()
    {
        _bookReviewCommandRepositoryMock = new Mock<IBookReviewCommandRepository>();
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateBookReviewCommandHandler(_bookReviewCommandRepositoryMock.Object, _bookCommandRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookExistsAndUserHasNotReviewed_ShouldCreateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, userId);
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _bookReviewCommandRepositoryMock
            .Setup(x => x.ExistsByUserAndBookAsync(command.UserId, command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(command.BookId);
        result.Value.UserId.ShouldBe(command.UserId);

        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewCommandRepositoryMock.Verify(x => x.ExistsByUserAndBookAsync(command.UserId, command.BookId, It.IsAny<CancellationToken>()), Times.Once);

        _bookReviewCommandRepositoryMock.Verify(x => x.AddAsync(
            It.Is<BookReview>(br =>
                br.Content == command.Content &&
                br.Rating.Value == command.Rating &&
                br.BookId == command.BookId &&
                br.UserId == command.UserId),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, userId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookReviewCommandRepositoryMock.Verify(x => x.ExistsByUserAndBookAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _bookReviewCommandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<BookReview>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserHasAlreadyReviewedBook_ShouldReturnConflictResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, userId);
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _bookReviewCommandRepositoryMock
            .Setup(x => x.ExistsByUserAndBookAsync(command.UserId, command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);

        _bookReviewCommandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<BookReview>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}