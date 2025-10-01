using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public class GetAllSolutionStacksHandler : IRequestHandler<GetAllSolutionStacks, PaginatedResponse<GetSolutionStackResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSolutionStacksHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<GetSolutionStackResponse>> Handle(
        GetAllSolutionStacks request,
        CancellationToken cancellationToken)
    {
        var query = _context.SolutionStacks.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            // TODO: Consider using full-text search for better performance on large datasets
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.Description.ToLower().Contains(searchTerm));
        }

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
        var solutionStacks = await query
            .Skip(request.Skip)
            .Take(request.GetValidPageSize())
            .Select(s => new GetSolutionStackResponse(
                s.Id,
                s.Name,
                s.Slug,
                s.Description,
                s.Category,
                s.Status,
                s.Owner,
                s.CreatedAt,
                s.UpdatedAt))
            .ToListAsync(cancellationToken);

        return PaginatedResponse<GetSolutionStackResponse>.Create(
            solutionStacks,
            request.GetValidPageNumber(),
            request.GetValidPageSize(),
            totalCount
        );
    }
}
