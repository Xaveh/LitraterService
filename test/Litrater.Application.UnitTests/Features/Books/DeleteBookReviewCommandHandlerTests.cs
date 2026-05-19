using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.DeleteBookReview;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly DeleteBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteBookReviewCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteBookReviewCommandHandler(_bookCommandRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), userId)!;
        var command = new DeleteBookReviewCommand(review.Id, userId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), originalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, adminUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(reviewId, userId);

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
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), originalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, differentUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        book.Reviews.Count.ShouldBe(1); // Review was not removed

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldDeleteAnyBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalUserId = Guid.NewGuid();
        var adminUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Another user's review", new Rating(3), originalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, adminUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}