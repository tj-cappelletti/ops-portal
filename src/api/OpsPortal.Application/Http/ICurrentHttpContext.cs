namespace OpsPortal.Application.Http;

public interface ICurrentHttpContext
{
    string? GetClaim(string claimType);

    string? GetIpAddress();

    string? GetRequestHeader(string headerName);

    string? GetSessionId();

    string? GetUserAgent();

    bool IsAuthenticated();
}
