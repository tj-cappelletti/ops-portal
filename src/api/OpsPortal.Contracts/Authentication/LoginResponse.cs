namespace OpsPortal.Contracts.Authentication;

public record LoginResponse
{
    public DateTime ExpiresAt { get; init; }

    public string Message { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public UserInfo User { get; init; } = null!;
}
