namespace OpsPortal.Application.Configuration;

public class RetentionPolicy
{
    public bool ArchiveBeforeDelete { get; set; } = false;

    public string? ArchiveLocation { get; set; }

    public int DaysToKeep { get; set; } = 90;
}
