namespace OpsPortal.Domain.Common.Auditing;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }

    string CreatedBy { get; }

    Guid CreatedById { get; }

    DateTime UpdatedAt { get; }

    string UpdatedBy { get; }

    Guid UpdatedById { get; }
}
