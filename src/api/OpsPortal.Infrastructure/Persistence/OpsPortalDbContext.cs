using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using OpsPortal.Application.Common.Interfaces;

namespace OpsPortal.Infrastructure.Persistence;

public class OpsPortalDbContext : DbContext, IApplicationDbContext
{
    private readonly IDatabaseProvider? _databaseProvider;

    public OpsPortalDbContext(
        DbContextOptions<OpsPortalDbContext> options,
        IDatabaseProvider? databaseProvider = null)
        : base(options)
    {
        _databaseProvider = databaseProvider;
    }

    public DbSet<SolutionStack> SolutionStacks => Set<SolutionStack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure SolutionStack entity
        modelBuilder.Entity<SolutionStack>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Use appropriate ID generation based on provider
            if (_databaseProvider?.IsSqlServer == true)
            {
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("NEWID()");
            }
            else if (_databaseProvider?.IsPostgreSql == true)
            {
                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()");
            }

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Slug)
                .IsUnique();

            entity.Property(e => e.Description)
                .HasMaxLength(500);

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

        // Seed data (works for all providers)
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            SeedData(modelBuilder);
        }
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed data implementation...
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Apply provider-specific type mappings
        if (_databaseProvider?.IsPostgreSql == true)
        {
            // PostgreSQL specific type mappings
            configurationBuilder.Properties<decimal>()
                .HaveColumnType("numeric(18,2)");
        }
        else if (_databaseProvider?.IsSqlServer == true)
        {
            // SQL Server specific type mappings
            configurationBuilder.Properties<decimal>()
                .HaveColumnType("decimal(18,2)");
        }
    }
}
