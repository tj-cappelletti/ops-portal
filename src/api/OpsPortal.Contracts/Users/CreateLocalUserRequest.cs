namespace OpsPortal.Contracts.Users;

public record CreateLocalUserRequest(
    string? AvatarUrl,
    string DisplayName,
    string? Email,
    string? FirstName,
    string Identifier,
    string? LastName,
    string? Locale,
    string Password,
    bool? RequirePasswordChange,
    string? TimeZone
);
