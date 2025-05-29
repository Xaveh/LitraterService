using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Litrater.Presentation.Common;

internal static class ResultsExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result) => result.Status switch
    {
        ResultStatus.Ok => Results.Ok(result.Value),
        ResultStatus.Created => Results.Created(),
        ResultStatus.NoContent => Results.NoContent(),
        ResultStatus.NotFound => Results.NotFound(),
        ResultStatus.Unauthorized => Results.Unauthorized(),
        ResultStatus.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
        ResultStatus.Conflict => Results.Conflict(),
        ResultStatus.Invalid => Results.BadRequest(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred.",
            Extensions =
            {
                ["errors"] = result.ValidationErrors
            }
        }),
        ResultStatus.Error => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred",
            Detail = GetErrorDetail(result.Errors, "An unexpected error occurred")
        }),
        ResultStatus.CriticalError => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "A critical error occurred",
            Detail = GetErrorDetail(result.Errors, "A critical error occurred while processing your request")
        }),
        ResultStatus.Unavailable => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Service Unavailable",
            Detail = GetErrorDetail(result.Errors, "The service is currently unavailable")
        }),
        _ => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = "An unexpected error occurred while processing your request"
        })
    };

    public static IResult ToHttpResult(this Result result) => result.Status switch
    {
        ResultStatus.Ok => Results.Ok(),
        ResultStatus.Created => Results.Created(),
        ResultStatus.NoContent => Results.NoContent(),
        ResultStatus.NotFound => Results.NotFound(),
        ResultStatus.Unauthorized => Results.Unauthorized(),
        ResultStatus.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
        ResultStatus.Conflict => Results.Conflict(),
        ResultStatus.Invalid => Results.BadRequest(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred.",
            Extensions =
                {
                    ["errors"] = result.ValidationErrors
                }
        }),
        ResultStatus.Error => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred",
            Detail = GetErrorDetail(result.Errors, "An unexpected error occurred")
        }),
        ResultStatus.CriticalError => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "A critical error occurred",
            Detail = GetErrorDetail(result.Errors, "A critical error occurred while processing your request")
        }),
        ResultStatus.Unavailable => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Service Unavailable",
            Detail = GetErrorDetail(result.Errors, "The service is currently unavailable")
        }),
        _ => Results.Problem(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = "An unexpected error occurred while processing your request"
        })
    };

    private static string GetErrorDetail(IEnumerable<string>? errors, string fallback)
    {
        return errors != null && errors.Any() ? string.Join("; ", errors) : fallback;
    }
}