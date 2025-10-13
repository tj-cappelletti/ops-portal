namespace OpsPortal.Application.Configuration;

public class AuditSettings
{
    public List<string> AlwaysAuditActions { get; set; } = new();

    public bool EnableAuditing { get; set; } = true;

    public bool EnableExternalAudit { get; set; } = false;

    public bool EnableQueryAuditing { get; set; } = false;

    public List<string> ExcludedActions { get; set; } = new();

    public string? ExternalAuditEndpoint { get; set; }

    public AuditLevel Level { get; set; } = AuditLevel.Standard;

    public RetentionPolicy Retention { get; set; } = new();
}
