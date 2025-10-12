namespace OpsPortal.Infrastructure.Persistence;

public class OpsPortalDatabaseProvider : IOpsPortalDatabaseProvider
{
    public bool IsPostgreSql => Provider is "postgresql" or "postgres" or "npgsql";

    public bool IsSqlite => Provider is "sqlite";

    public bool IsSqlServer => Provider is "sqlserver" or "mssql";

    public string Provider { get; }

    public OpsPortalDatabaseProvider(string? provider)
    {
        Provider = provider?.ToLowerInvariant() ?? "postgresql";
    }
}
