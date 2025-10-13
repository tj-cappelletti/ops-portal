namespace OpsPortal.Contracts.Authentication;

public record LoginRequest
{
    public string Identifier { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
