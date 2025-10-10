using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Constants;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Persistence.ModelBuilders;

internal class SolutionStackStatusModelBuilder : IModelBuilder
{
    private readonly IDatabaseProvider? _databaseProvider;

    public SolutionStackStatusModelBuilder(IDatabaseProvider? databaseProvider = null)
    {
        _databaseProvider = databaseProvider;
    }

    public void BuildModel(ModelBuilder modelBuilder)
    {
        // TODO: Inject the authentication type from the configuration
        var systemStatuses = SystemDefaults.Statuses.GetAllSeedData()
            .Select(SolutionStackStatus.CreateSystemStatus)
            .ToArray();

        modelBuilder.Entity<SolutionStackStatus>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Use appropriate ID generation based on provider
            if (_databaseProvider?.IsSqlServer == true)
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");
            else if (_databaseProvider?.IsPostgreSql == true)
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Slug)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(100);

            entity.HasIndex(e => e.Slug)
                .IsUnique();

            // Provider-specific Description length
            if (_databaseProvider?.IsSqlServer == true)
                // NVARCHAR(MAX) for SQL Server
                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)");
            else if (_databaseProvider?.IsPostgreSql == true)
                // TEXT for PostgreSQL
                entity.Property(e => e.Description)
                    .HasColumnType("text");

            // One-to-many relationship with SolutionStack
            entity.HasMany(e => e.SolutionStacks)
                .WithOne(s => s.Status)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasData(systemStatuses);
        });
    }
}
