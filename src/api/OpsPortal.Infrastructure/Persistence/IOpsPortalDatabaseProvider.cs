namespace OpsPortal.Infrastructure.Persistence;

public interface IOpsPortalDatabaseProvider
{
    bool IsPostgreSql { get; }

    bool IsSqlite { get; }

    bool IsSqlServer { get; }

    string Provider { get; }
}
