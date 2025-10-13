namespace OpsPortal.Contracts.Authentication;

public record RefreshTokenResponse
{
    public DateTime ExpiresAt { get; init; }

    public string Token { get; init; } = string.Empty;
}
