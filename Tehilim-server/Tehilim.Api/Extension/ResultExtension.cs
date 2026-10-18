using Microsoft.AspNetCore.Mvc;
using Tehilim.Core.DTOs.Responds;
using Tehilim.Core.Results;

namespace Tehilim.API.Extension;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess ? new NoContentResult() : ToError(result);

    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess ? new OkObjectResult(result.Value) : ToError(result);

    private static IActionResult ToError(Result result)
    {
        var status = result.Error switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => throw new InvalidOperationException($"Unhandled error type: {result.Error}")
        };

        return new ErrorResult(status, result.Message ?? string.Empty);
    }

    private sealed class ErrorResult(int status, string message) : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context) =>
            new ObjectResult(new ErrorResponse(status, message, context.HttpContext.TraceIdentifier))
            {
                StatusCode = status
            }.ExecuteResultAsync(context);
    }
}