namespace Tehilim.Core.DTOs.Responds
{
    public record ErrorResponse(
        int Status,
        string Message,
        string CorrelationId,
        IDictionary<string, string[]>? Errors = null);
}