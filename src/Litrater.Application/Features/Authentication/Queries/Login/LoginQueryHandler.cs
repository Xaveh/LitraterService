using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authentication.Queries.Login;

internal sealed class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider) : IQueryHandler<LoginQuery, string>
{
    public async Task<Result<string>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(query.Password, user.PasswordHash))
        {
            return Result.Unauthorized();
        }

        if (!user.IsActive)
        {
            return Result.Forbidden();
        }

        var token = tokenProvider.GenerateToken(user);

        return Result<string>.Success(token);
    }
}
