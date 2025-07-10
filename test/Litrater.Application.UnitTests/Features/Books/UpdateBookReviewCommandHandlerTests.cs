using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.UpdateBookReview;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookReviewCommandHandlerTests
{
    private readonly Mock<IBookReviewRepository> _bookReviewRepositoryMock;
    private readonly UpdateBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateBookReviewCommandHandlerTests()
    {
        _bookReviewRepositoryMock = new Mock<IBookReviewRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateBookReviewCommandHandler(_bookReviewRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new UpdateBookReviewCommand(bookReviewId, "Updated content", 4, userId);
        var existingBookReview = new BookReview(bookReviewId, "Original content", 5, bookId, userId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(userId);

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new UpdateBookReviewCommand(bookReviewId, "Admin updated content", 3, adminUserId, true);
        var existingBookReview = new BookReview(bookReviewId, "Original content", 5, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalUserId); // Original user ID should remain

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateBookReviewCommand(bookReviewId, "Updated content", 4, userId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookReview?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
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
        var command = new UpdateBookReviewCommand(bookReviewId, "Unauthorized update", 4, differentUserId);
        var existingBookReview = new BookReview(bookReviewId, "Original content", 5, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookReviewId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var command = new UpdateBookReviewCommand(bookReviewId, "Admin override update", 2, adminUserId, true);
        var existingBookReview = new BookReview(bookReviewId, "Original content", 5, bookId, originalUserId);

        _bookReviewRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBookReview);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalUserId); // Original user ID should remain

        _bookReviewRepositoryMock.Verify(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}