using System.Security.Claims;
using Keycloak.AuthServices.Common.Claims;

namespace Litrater.Presentation.Extensions;

internal static class ClaimsPrincipalExtensions
{
    internal static bool HasResourceRole(this ClaimsPrincipal principal, string role)
    {
        return principal.Claims.TryGetResourceCollection(out var resourceCollection) &&
               resourceCollection.Values.Any(resource => resource.Roles.Contains(role));
    }
}