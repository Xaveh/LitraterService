using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : ICommand;