namespace OpsPortal.Infrastructure.Persistence;

public interface IDatabaseProvider
{
    string Provider { get; }
    bool IsPostgreSql { get; }
    bool IsSqlServer { get; }
    bool IsSqlite { get; }
}

public class DatabaseProvider : IDatabaseProvider
{
    public string Provider { get; }

    public DatabaseProvider(string provider)
    {
        Provider = provider?.ToLowerInvariant() ?? "postgresql";
    }

    public bool IsPostgreSql => Provider is "postgresql" or "postgres" or "npgsql";
    public bool IsSqlServer => Provider is "sqlserver" or "mssql";
    public bool IsSqlite => Provider is "sqlite";
}
