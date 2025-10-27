using OpsPortal.WebApi.Middleware;

namespace OpsPortal.WebApi.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    ///     Gets the correlation ID from the current request.
    ///     Throws InvalidOperationException if not found (indicates middleware not configured).
    /// </summary>
    public static string GetCorrelationId(this HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdContextKey, out var correlationId) &&
            correlationId is string id)
            return id;

        throw new InvalidOperationException(
            $"{CorrelationIdMiddleware.CorrelationIdContextKey} not found in HttpContext.Items. " +
            "Ensure CorrelationIdMiddleware is registered in the pipeline.");
    }

    /// <summary>
    ///     Tries to get the correlation ID from the current request.
    /// </summary>
    public static bool TryGetCorrelationId(this HttpContext context, out string? correlationId)
    {
        if (context.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdContextKey, out var value) && value is string id)
        {
            correlationId = id;
            return true;
        }

        correlationId = null;
        return false;
    }
}
