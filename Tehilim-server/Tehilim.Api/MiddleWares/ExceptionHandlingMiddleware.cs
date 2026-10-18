using Tehilim.Core.DTOs.Responds;

namespace Tehilim.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // הלקוח ניתק לפני שהתשובה הושלמה: אין למי להחזיר תשובה, וזו אינה שגיאה
        }
        catch (Exception ex)
        {
            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = context.TraceIdentifier }))
            {
                logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers[CorrelationIdMiddleware.HeaderName] = context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(new ErrorResponse(
                StatusCodes.Status500InternalServerError,
                "אירעה שגיאה בשרת. נסה שוב מאוחר יותר",
                context.TraceIdentifier));
        }
    }
}