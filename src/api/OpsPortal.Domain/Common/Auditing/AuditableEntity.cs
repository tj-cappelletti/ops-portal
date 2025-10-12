namespace OpsPortal.Domain.Common.Auditing;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime CreatedAt { get; protected set; }

    public string CreatedBy { get; protected set; } = string.Empty;

    public Guid CreatedById { get; protected set; }

    public DateTime UpdatedAt { get; protected set; }

    public string UpdatedBy { get; protected set; } = string.Empty;

    public Guid UpdatedById { get; protected set; }

    public void SetCreatedAudit(string userIdentifier, Guid userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userIdentifier;
        CreatedById = userId;
        UpdatedAt = CreatedAt;
        UpdatedBy = userIdentifier;
        UpdatedById = userId;
    }

    public void SetUpdatedAudit(string userIdentifier, Guid userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userIdentifier;
        UpdatedById = userId;
    }
}
