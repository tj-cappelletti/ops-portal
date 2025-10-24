namespace OpsPortal.Application.Auditing;

public record ActorInfo(
    Guid ActorId,
    string ActorDisplayName,
    string ActorIdentifier,
    string ActorEmail);
