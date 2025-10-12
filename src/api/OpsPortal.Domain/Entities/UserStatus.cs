namespace OpsPortal.Domain.Entities;

/// <summary>
/// User account status enumeration.
/// </summary>
/// <remarks>
/// ⚠️ CRITICAL: Never change existing enum values once deployed to production/main branch!
/// ⚠️ Always assign explicit integer values!
/// ⚠️ Only add new values at the end with next available number!
/// </remarks>
public enum UserStatus
{
    /// <summary>User account is active and can log in</summary>
    Active = 1,

    /// <summary>User account is inactive but can be reactivated</summary>
    Inactive = 2,

    /// <summary>User account is locked due to security policy</summary>
    Locked = 3,

    /// <summary>User account is soft-deleted</summary>
    Deleted = 4,

    /// <summary>User account created but email not verified</summary>
    PendingActivation = 5,

    /// <summary>User account temporarily suspended</summary>
    Suspended = 6
}
