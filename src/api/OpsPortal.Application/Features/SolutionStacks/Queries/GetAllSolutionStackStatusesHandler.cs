using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public class GetAllSolutionStackStatusesHandler : IRequestHandler<GetAllSolutionStackStatuses, PaginatedResponse<GetSolutionStackStatusResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSolutionStackStatusesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<GetSolutionStackStatusResponse>> Handle(GetAllSolutionStackStatuses request, CancellationToken cancellationToken)
    {
        var query = _context.SolutionStackStatuses.AsQueryable();

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name),
            _ => query.OrderBy(s => s.Name)
        };

        // Apply pagination
        var solutionStackStatuses = await query
            .Skip(request.Skip)
            .Take(request.GetValidPageSize())
            .Select(s => new GetSolutionStackStatusResponse(
                s.Id,
                s.Name,
                s.Slug,
                s.Description))
            .ToListAsync(cancellationToken);

        return PaginatedResponse<GetSolutionStackStatusResponse>.Create(
            solutionStackStatuses,
            request.GetValidPageNumber(),
            request.GetValidPageSize(),
            totalCount);
    }
}
