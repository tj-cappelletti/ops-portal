using Microsoft.EntityFrameworkCore;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SolutionStack> SolutionStacks { get; }

    DbSet<SolutionStackStatus> SolutionStackStatuses { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
