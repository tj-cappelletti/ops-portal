namespace OpsPortal.Domain.Common.Auditing;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime CreatedAt { get; protected set; }

    public string CreatedBy { get; protected set; } = string.Empty;

    public Guid CreatedById { get; protected set; }

    public DateTime UpdatedAt { get; protected set; }

    public string UpdatedBy { get; protected set; } = string.Empty;

    public Guid UpdatedById { get; protected set; }

    internal void SetCreatedAudit(string userIdentifier, Guid userId, DateTime timestamp)
    {
        CreatedAt = timestamp;
        CreatedBy = userIdentifier;
        CreatedById = userId;
        UpdatedAt = timestamp;
        UpdatedBy = userIdentifier;
        UpdatedById = userId;
    }

    public void SetCreatedAudit(string userIdentifier, Guid userId)
    {
        SetCreatedAudit(userIdentifier, userId, DateTime.UtcNow);
    }

    internal void SetUpdatedAudit(string userIdentifier, Guid userId, DateTime timestamp)
    {
        UpdatedAt = timestamp;
        UpdatedBy = userIdentifier;
        UpdatedById = userId;
    }

    public void SetUpdatedAudit(string userIdentifier, Guid userId)
    {
        SetUpdatedAudit(userIdentifier, userId, DateTime.UtcNow);
    }
}
