namespace OpsPortal.Domain.Entities;

public enum AuditEventType
{
    Authentication,
    Command,
    Query,
    DataModification,
    ConfigurationChange,
    SecurityEvent,
    SystemEvent,
    IntegrationEvent
}
