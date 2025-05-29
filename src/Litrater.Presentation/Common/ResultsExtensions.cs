using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Litrater.Presentation.Common;

internal static class ResultsExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
        => ToHttpResultCore(result.Status, result.Errors, result.ValidationErrors, result.Value);

    public static IResult ToHttpResult(this Result result)
        => ToHttpResultCore(result.Status, result.Errors, result.ValidationErrors);

    private static IResult ToHttpResultCore(ResultStatus status, IEnumerable<string>? errors, IEnumerable<ValidationError>? validationErrors, object? value = null)
    {
        return status switch
        {
            ResultStatus.Ok => value is not null ? Results.Ok(value) : Results.Ok(),
            ResultStatus.Created => Results.Created(),
            ResultStatus.NoContent => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.Unauthorized => Results.Unauthorized(),
            ResultStatus.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
            ResultStatus.Conflict => Results.Conflict(),
            ResultStatus.Invalid => Results.Problem(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    ["errors"] = validationErrors
                }
            }),
            ResultStatus.Error => Results.Problem(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred",
                Detail = "An unexpected error occurred",
                Extensions =
                {
                    ["errors"] = errors
                }
            }),
            ResultStatus.CriticalError => Results.Problem(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "A critical error occurred",
                Detail = "A critical error occurred while processing your request",
                Extensions =
                {
                    ["errors"] = errors
                }
            }),
            ResultStatus.Unavailable => Results.Problem(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4",
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Service Unavailable",
                Detail = "The service is currently unavailable",
                Extensions =
                {
                    ["errors"] = errors
                }
            }),
            _ => Results.Problem(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred",
                Detail = "An unexpected error occurred while processing your request",
                Extensions =
                {
                    ["errors"] = errors
                }
            })
        };
    }
}