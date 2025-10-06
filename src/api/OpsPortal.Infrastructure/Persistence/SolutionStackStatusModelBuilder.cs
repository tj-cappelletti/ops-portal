using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Persistence;

internal class SolutionStackStatusModelBuilder : IModelBuilder
{
    private readonly IDatabaseProvider? _databaseProvider;

    public SolutionStackStatusModelBuilder(IDatabaseProvider? databaseProvider = null)
    {
        _databaseProvider = databaseProvider;
    }

    public void BuildModel(ModelBuilder modelBuilder)
    {
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

                entity.HasData(
                    SolutionStackStatus.CreateSystemStatus(
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        "Active",
                        "The solution stack is active and available for use.",
                        "active"),
                    SolutionStackStatus.CreateSystemStatus(
                        Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        "Inactive",
                        "The solution stack is inactive and not available for use.",
                        "inactive")
            );
        });
    }
}
