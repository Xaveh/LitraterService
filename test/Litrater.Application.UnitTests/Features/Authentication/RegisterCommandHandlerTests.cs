using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authentication.Commands.Register;
using Litrater.Domain.Users;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authentication;

public sealed class RegisterCommandHandlerTests
{
    private readonly RegisterCommandHandler _handler;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RegisterCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "John", "Doe");
        const string hashedPassword = "hashed_password";

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.Hash(command.Password))
            .Returns(hashedPassword);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Email.ShouldBe(command.Email);
        result.Value.FirstName.ShouldBe(command.FirstName);
        result.Value.LastName.ShouldBe(command.LastName);

        _userRepositoryMock.Verify(x => x.AddAsync(
            It.Is<User>(u =>
                u.Email == command.Email &&
                u.PasswordHash == hashedPassword &&
                u.FirstName == command.FirstName &&
                u.LastName == command.LastName &&
                u.IsActive),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyExists_ShouldReturnConflictResult()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "John", "Doe");

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);
        result.Errors.ShouldContain("User with this email already exists");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "password123", "John", "Doe");
        const string hashedPassword = "hashed_password";

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(x => x.Hash(command.Password))
            .Returns(hashedPassword);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(x => x.Hash(command.Password), Times.Once);
    }
}