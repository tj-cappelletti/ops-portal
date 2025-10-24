using System.Text.Json;
using System.Text.RegularExpressions;
using OpsPortal.Domain.Common.Auditing;
using OpsPortal.Domain.Constants;

namespace OpsPortal.Domain.Entities;

public class User : AuditableEntity
{
    public string? AvatarUrl { get; private set; }

    public string DisplayName { get; private set; }

    public string? Email { get; private set; }

    public string? ExternalId { get; private set; }

    public string? ExternalMetadata { get; private set; } // JSON for provider-specific data

    public int FailedLoginAttempts { get; private set; }

    public string? FirstName { get; private set; }

    public string Identifier { get; private init; } // Username or Email depending on authentication mode

    public string? IdentityProvider { get; private set; }

    public bool IsDeleted { get; private set; } // Soft delete flag

    public bool IsLocked { get; private set; }

    public bool IsSystemUser { get; private set; } // True for built-in system accounts

    public DateTime? LastLoginAt { get; private set; }

    public string? LastLoginIp { get; private set; }

    public string? LastName { get; private set; }

    public string? Locale { get; private set; }

    public DateTime? LockedUntil { get; private set; }

    public int LoginCount { get; private set; }

    public DateTime? PasswordChangedAt { get; private set; }

    public string? PasswordHash { get; private set; }

    public string? Preferences { get; private set; } = null; // JSON for UI preferences

    public string? RefreshToken { get; private set; } = null;

    public DateTime? RefreshTokenExpiry { get; private set; } = null;

    public bool? RequirePasswordChange { get; private set; }

    public UserStatus Status { get; private set; }

    public string? TimeZone { get; private set; } = null;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private User() { } // EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    // Factory method for local users - flexible identifier
    public static User CreateLocalUser(
        string identifier,
        string? email,
        string displayName,
        string passwordHash,
        bool requirePasswordChange,
        User createdByUser)
    {
        var timestamp = DateTime.UtcNow;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Identifier = identifier.ToLowerInvariant(),
            Email = email?.ToLowerInvariant(),
            DisplayName = displayName,
            PasswordChangedAt = timestamp,
            PasswordHash = passwordHash,
            RequirePasswordChange = requirePasswordChange,
            Status = UserStatus.Active,
            IsLocked = false
        };

        user.SetCreatedAudit(createdByUser.Identifier, createdByUser.Id, timestamp);

        return user;
    }

    // Factory method for SSO users - email required
    //public static User CreateSsoUser(
    //    string email,
    //    string displayName,
    //    string externalId,
    //    string identityProvider,
    //    Dictionary<string, object>? claims = null)
    //{
    //    var user = new User
    //    {
    //        Id = Guid.NewGuid(),
    //        Identifier = email.ToLowerInvariant(), // For SSO, identifier = email
    //        Email = email.ToLowerInvariant(),
    //        DisplayName = displayName,
    //        ExternalId = externalId,
    //        IdentityProvider = identityProvider,
    //        Status = UserStatus.Active,
    //        IsLocked = false
    //    };

    //    return user;
    //}


    public static User CreateSystemUser(
        Guid id,
        string identifier,
        string email,
        string displayName)
    {
        return new User
        {
            Id = id,
            Identifier = identifier,
            Email = email,
            DisplayName = displayName,
            IsSystemUser = true,
            Status = UserStatus.Active,
            IsLocked = true, // Prevent login
            PasswordHash = null, // No password
            CreatedAt = SystemDefaults.BaseDateTime,
            CreatedBy = email, // Self-created
            CreatedById = id,
            UpdatedAt = SystemDefaults.BaseDateTime,
            UpdatedBy = email, // Self-updated
            UpdatedById = id
        };
    }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void LockAccount(TimeSpan duration)
    {
        IsLocked = true;
        LockedUntil = DateTime.UtcNow.Add(duration);
    }

    public void RecordFailedLoginAttempt()
    {
        FailedLoginAttempts++;
    }

    public void RecordLogin(string? ipAddress)
    {
        LastLoginAt = DateTime.UtcNow;
        LastLoginIp = ipAddress;
        LoginCount++;
        FailedLoginAttempts = 0;
        IsLocked = false;
        LockedUntil = null;
    }

    public void Restore()
    {
        IsDeleted = false;
    }

    public void Unlock()
    {
        IsLocked = false;
        LockedUntil = null;
        FailedLoginAttempts = 0;
    }

    public void SetRequirePasswordChange()
    {
        RequirePasswordChange = true;
    }

    public void UpdatePasswordHash(string newHash)
    {
        PasswordHash = newHash;
        PasswordChangedAt = DateTime.UtcNow;
        RequirePasswordChange = false;
    }
}
