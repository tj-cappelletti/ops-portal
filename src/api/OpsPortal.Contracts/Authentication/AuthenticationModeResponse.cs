namespace OpsPortal.Contracts.Authentication;

public record AuthenticationModeResponse
{
    public string Mode { get; init; } = string.Empty;

    public bool SsoEnabled { get; init; }

    public string? SsoLoginUrl { get; init; }
}
