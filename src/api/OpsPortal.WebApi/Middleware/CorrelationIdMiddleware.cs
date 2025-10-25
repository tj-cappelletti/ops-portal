namespace OpsPortal.WebApi.Middleware;

public class CorrelationIdMiddleware
{
    public const string CorrelationIdContextKey = "CorrelationId";
    public const string CorrelationIdHeaderName = "X-Correlation-Id";
    public const string RequestIdHeaderName = "X-Request-Id";
    public const string TraceIdContextKey = "TraceId";

    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if client sent a correlation ID (for distributed tracing)
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
                            ?? context.Request.Headers["X-Request-Id"].FirstOrDefault()
                            ?? context.TraceIdentifier; // Fall back to ASP.NET Core's trace ID

        // Ensure we have a correlation ID
        if (string.IsNullOrWhiteSpace(correlationId)) correlationId = Guid.NewGuid().ToString();

        // Store in HttpContext Items for use throughout the request
        context.Items["CorrelationId"] = correlationId;

        // Add to response headers immediately
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
            context.Response.Headers[RequestIdHeaderName] = context.TraceIdentifier;
            return Task.CompletedTask;
        });

        // Add to logging scope
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   [CorrelationIdContextKey] = correlationId,
                   [TraceIdContextKey] = context.TraceIdentifier
               }))
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request failed with correlation ID {CorrelationId}", correlationId);
                throw;
            }
        }
    }
}
