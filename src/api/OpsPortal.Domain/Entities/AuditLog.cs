using System.Text.Json;

namespace OpsPortal.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public AuditEventType EventType { get; private set; }
    public string EntityType { get; private set; }
    public string? EntityId { get; private set; }
    public string Action { get; private set; }
    public string? OldValues { get; private set; } // JSON
    public string? NewValues { get; private set; } // JSON
    public string? Changes { get; private set; } // JSON diff
    public string? Metadata { get; private set; } // Additional context
    public string IpAddress { get; private set; }
    public string? CorrelationId { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorDetails { get; private set; }
    
    public static AuditLog CreateCommandAudit(
        string userId,
        string userEmail,
        string commandName,
        object commandData,
        string ipAddress)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            UserEmail = userEmail,
            EventType = AuditEventType.Command,
            Action = commandName,
            NewValues = JsonSerializer.Serialize(commandData),
            IpAddress = ipAddress,
            Success = true
        };
    }
}
