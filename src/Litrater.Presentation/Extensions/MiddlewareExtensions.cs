using Litrater.Presentation.Middlewares;

namespace Litrater.Presentation.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestContextLoggingMiddleware>();
    }

    public static IApplicationBuilder UseUserSync(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserSyncMiddleware>();
    }
}