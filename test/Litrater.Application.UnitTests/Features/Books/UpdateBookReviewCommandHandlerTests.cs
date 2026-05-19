using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.UpdateBookReview;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly UpdateBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateBookReviewCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateBookReviewCommandHandler(_bookCommandRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), userId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Updated content", 4, userId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(userId);

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Admin updated content", 3, adminUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalUserId); // Original user ID should remain

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new UpdateBookReviewCommand(reviewId, "Updated content", 4, userId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerAndNotAdmin_ShouldReturnForbiddenResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Unauthorized update", 4, differentUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Admin override update", 2, adminUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalUserId); // Original user ID should remain

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}