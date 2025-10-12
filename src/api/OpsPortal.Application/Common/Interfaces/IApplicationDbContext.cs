using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<AuditLog> AuditLogs { get; }

    //DbSet<SolutionStack> SolutionStacks { get; }

    //DbSet<SolutionStackStatus> SolutionStackStatuses { get; }

    //DbSet<Tag> Tags { get; }

    DbSet<User> Users { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
