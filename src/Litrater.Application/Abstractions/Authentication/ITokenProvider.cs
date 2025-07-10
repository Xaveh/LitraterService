using Litrater.Domain.Users;

namespace Litrater.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateToken(User user);
}