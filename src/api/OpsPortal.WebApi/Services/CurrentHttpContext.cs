using OpsPortal.Application.Http;

namespace OpsPortal.WebApi.Services;

public class CurrentHttpContext : ICurrentHttpContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentHttpContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetClaim(string claimType)
    {
        // TODO: Implement claim retrieval
        return null;
    }

    public string? GetIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null) return null;

        var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        
        return !string.IsNullOrEmpty(forwarded)
            ? forwarded.Split(',').First().Trim()
            : httpContext.Connection.RemoteIpAddress?.ToString();
    }

    public string? GetRequestHeader(string headerName)
    {
        return _httpContextAccessor.HttpContext?.Request.Headers[headerName].ToString();
    }

    public string? GetSessionId()
    {
        return _httpContextAccessor.HttpContext?.Session.Id;
    }

    public string? GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
    }

    public bool IsAuthenticated()
    {
        // TODO: Implement authentication check
        return false;
    }
}
