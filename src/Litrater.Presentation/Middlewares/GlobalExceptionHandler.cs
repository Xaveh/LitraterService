using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Litrater.Presentation.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var requestPath = httpContext.Request.Path;
        var requestMethod = httpContext.Request.Method;
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        logger.LogError(exception,
            "Unhandled exception occurred for {RequestMethod} {RequestPath}. " +
            "User: {UserId} ({UserEmail}), IP: {RemoteIpAddress}, UserAgent: {UserAgent}",
            requestMethod, requestPath, userId ?? "Anonymous", userEmail ?? "Anonymous",
            remoteIpAddress, userAgent);

        var problemDetails = exception switch
        {
            BadHttpRequestException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request"
            },
            JsonException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid JSON"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "Server Error",
                Detail = "An unexpected error occurred while processing your request."
            }
        };

        if (environment.IsDevelopment())
        {
            problemDetails.Detail = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        }

        if (problemDetails.Status.HasValue)
        {
            httpContext.Response.StatusCode = problemDetails.Status.Value;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}