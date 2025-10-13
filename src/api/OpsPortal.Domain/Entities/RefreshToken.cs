using OpsPortal.Domain.Common;

namespace OpsPortal.Domain.Entities;

public class RefreshToken : Entity
{
    public DateTime CreatedAt { get; private init; }

    public DateTime ExpiresAt { get; private init; }

    public string? IpAddress { get; private init; }

    public bool IsActive => !IsRevoked && !IsExpired;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public string? RevokedReason { get; private set; }

    public string Token { get; private init; } = string.Empty;

    public User User { get; private init; } = null!;

    public string? UserAgent { get; private init; }

    public Guid UserId { get; private init; }

    public static RefreshToken Create(
        Guid userId,
        string token,
        DateTime expiresAt,
        string? userAgent = null,
        string? ipAddress = null)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            IsRevoked = false
        };
    }

    public void Revoke(string reason)
    {
        IsRevoked = true;
        RevokedReason = reason;
        RevokedAt = DateTime.UtcNow;
    }
}
