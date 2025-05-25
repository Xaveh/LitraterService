using Litrater.Presentation.Common.Interfaces;

namespace Litrater.Presentation.Common;

internal static class EndpointExtensions
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointTypes = typeof(EndpointExtensions).Assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(type)!;
            endpoint.MapEndpoint(app);
        }
    }
}