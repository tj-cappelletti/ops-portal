using OpsPortal.Domain.Entities;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users;

public static class UserExtensions
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(
            user.AvatarUrl,
            user.DisplayName,
            user.Email,
            user.ExternalId,
            user.ExternalMetadata,
            user.FailedLoginAttempts,
            user.FirstName,
            user.Id,
            user.Identifier,
            user.IdentityProvider,
            user.IsDeleted,
            user.IsLocked,
            user.IsSystemUser,
            user.LastLoginAt,
            user.LastLoginIp,
            user.LastName,
            user.Locale,
            user.LockedUntil,
            user.LoginCount,
            user.PasswordChangedAt,
            user.Preferences,
            user.RefreshTokenExpiry,
            user.RequirePasswordChange,
            user.Status.ToString(),
            user.TimeZone
        );
    }
}
