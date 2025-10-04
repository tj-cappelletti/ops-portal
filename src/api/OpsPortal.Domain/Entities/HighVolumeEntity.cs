namespace OpsPortal.Domain.Entities;

public abstract class HighVolumeEntity
{
    public long Id { get; protected set; } // Internal PK for performance
    public Guid PublicId { get; protected set; } = Guid.NewGuid(); // External reference
}
