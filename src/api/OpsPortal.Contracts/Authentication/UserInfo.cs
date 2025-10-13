namespace OpsPortal.Contracts.Authentication;

public record UserInfo
{
    public string? AvatarUrl { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public Guid Id { get; init; }

    public string Identifier { get; init; } = string.Empty;

    public List<string> Roles { get; init; } = new();
}
