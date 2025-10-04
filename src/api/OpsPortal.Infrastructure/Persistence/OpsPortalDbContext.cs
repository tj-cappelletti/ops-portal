using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Persistence;

public class OpsPortalDbContext : DbContext, IApplicationDbContext
{
    private readonly IDatabaseProvider? _databaseProvider;

    public DbSet<SolutionStack> SolutionStacks => Set<SolutionStack>();

    public DbSet<SolutionStackStatus> SolutionStackStatuses => Set<SolutionStackStatus>();

    public OpsPortalDbContext(
        DbContextOptions<OpsPortalDbContext> options,
        IDatabaseProvider? databaseProvider = null)
        : base(options)
    {
        _databaseProvider = databaseProvider;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Apply provider-specific type mappings
        if (_databaseProvider?.IsPostgreSql == true)
            // PostgreSQL specific type mappings
            configurationBuilder.Properties<decimal>()
                .HaveColumnType("numeric(18,2)");
        else if (_databaseProvider?.IsSqlServer == true)
            // SQL Server specific type mappings
            configurationBuilder.Properties<decimal>()
                .HaveColumnType("decimal(18,2)");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Entities using Reflection and IModelBuilder implementations
        var modelBuilderTypes = typeof(IModelBuilder).Assembly.GetTypes()
            .Where(t => typeof(IModelBuilder).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in modelBuilderTypes)
        {
            var instance = (IModelBuilder?)Activator.CreateInstance(type, _databaseProvider);
            instance?.BuildModel(modelBuilder);
        }

        // Seed data (works for all providers)
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed data implementation...
    }
}
