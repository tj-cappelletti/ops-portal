namespace OpsPortal.Contracts.Users;

public record UserResponse(
    string? AvatarUrl,
    string DisplayName,
    string? Email,
    string? ExternalId,
    string? ExternalMetadata,
    int FailedLoginAttempts,
    string? FirstName,
    Guid Id,
    string Identifier,
    string? IdentityProvider,
    bool IsDeleted,
    bool IsLocked,
    bool IsSystemUser,
    DateTime? LastLoginAt,
    string? LastLoginIp,
    string? LastName,
    string? Locale,
    DateTime? LockedUntil,
    int LoginCount,
    DateTime? PasswordChangedAt,
    string? Preferences,
    DateTime? RefreshTokenExpiry,
    bool? RequirePasswordChange,
    string Status,
    string? TimeZone
);
