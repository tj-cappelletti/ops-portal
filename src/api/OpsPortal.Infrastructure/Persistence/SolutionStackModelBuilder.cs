using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Persistence;

internal class SolutionStackModelBuilder : IModelBuilder
{
    private readonly IDatabaseProvider? _databaseProvider;

    public SolutionStackModelBuilder(IDatabaseProvider? databaseProvider = null)
    {
        _databaseProvider = databaseProvider;
    }

    public void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SolutionStack>(entity =>
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
                .HasMaxLength(300);

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

            // Handle datetime precision differences
            if (_databaseProvider?.IsSqlServer == true)
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime2");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime2");
            }
            else if (_databaseProvider?.IsPostgreSql == true)
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone");
            }
        });
    }
}
