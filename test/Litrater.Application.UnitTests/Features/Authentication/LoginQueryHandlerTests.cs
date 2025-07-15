using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Features.Authentication.Queries.Login;
using Litrater.Domain.Users;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authentication;

public sealed class LoginQueryHandlerTests
{
    private readonly LoginQueryHandler _handler;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenProvider> _tokenProviderMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public LoginQueryHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenProviderMock = new Mock<ITokenProvider>();
        _handler = new LoginQueryHandler(_userRepositoryMock.Object, _passwordHasherMock.Object, _tokenProviderMock.Object, new Mock<ILogger<LoginQueryHandler>>().Object);
    }

    [Fact]
    public async Task Handle_WhenUserExistsAndPasswordIsCorrectAndUserIsActive_ShouldReturnToken()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "password123");
        var user = new User(Guid.NewGuid(), query.Email, "hashedPassword", "John", "Doe");
        const string expectedToken = "jwt_token";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.Verify(query.Password, user.PasswordHash))
            .Returns(true);

        _tokenProviderMock
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnUnauthorizedResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "password123");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _passwordHasherMock.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsIncorrect_ShouldReturnUnauthorizedResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "wrongpassword");
        var user = new User(Guid.NewGuid(), query.Email, "hashedPassword", "John", "Doe");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.Verify(query.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);
        _tokenProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotActive_ShouldReturnForbiddenResult()
    {
        // Arrange
        var query = new LoginQuery("test@example.com", "password123");
        var user = new User(Guid.NewGuid(), query.Email, "hashedPassword", "John", "Doe", isActive: false);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.Verify(query.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        _tokenProviderMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}