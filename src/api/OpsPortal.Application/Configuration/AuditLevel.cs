namespace OpsPortal.Application.Configuration;

public enum AuditLevel
{
    Minimal, // Auth events only (homelab)
    Standard, // Commands and config changes (default)
    Detailed, // Include sensitive queries
    Compliance // Everything including read operations
}
