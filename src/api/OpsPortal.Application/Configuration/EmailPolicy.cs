namespace OpsPortal.Application.Configuration;

public class EmailPolicy
{
    public List<string> AllowedDomains { get; set; } = []; // Empty = all allowed

    public List<string> BlockedDomains { get; set; } = [];

    public bool RequireEmailVerification { get; set; } = false;

    public bool RequireOrganizationalEmail { get; set; } = false;
}
