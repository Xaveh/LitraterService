using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authentication.Queries.Login;

public sealed record LoginQuery(string Email, string Password) : IQuery;