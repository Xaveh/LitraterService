using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.DeleteBookReview;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookReviewCommandHandlerTests
{
    private readonly Mock<IBookReviewRepository> _bookReviewRepositoryMock;
    private readonly DeleteBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteBookReviewCommandHandlerTests()
    {
        _bookReviewRepositoryMock = new Mock<IBookReviewRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteBookReviewCommandHandler(_bookReviewRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(bookReviewId, userId);
        var existingBookReview = new BookReview(bookReviewId, "Test content", 5, bookId, userId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewRepositoryMock.Verify(x => x.Delete(existingBookReview), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(bookReviewId, adminUserId, true);
        var existingBookReview = new BookReview(bookReviewId, "Test content", 5, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewRepositoryMock.Verify(x => x.Delete(existingBookReview), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(bookReviewId, userId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookReview?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewRepositoryMock.Verify(x => x.Delete(It.IsAny<BookReview>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerAndNotAdmin_ShouldReturnForbiddenResult()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(bookReviewId, differentUserId);
        var existingBookReview = new BookReview(bookReviewId, "Test content", 5, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewRepositoryMock.Verify(x => x.Delete(It.IsAny<BookReview>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldDeleteAnyBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(bookReviewId, adminUserId, true);
        var existingBookReview = new BookReview(bookReviewId, "Another user's review", 3, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bookReviewRepositoryMock.Verify(x => x.Delete(existingBookReview), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}