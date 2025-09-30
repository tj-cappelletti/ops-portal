using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Common.Extensions;
using OpsPortal.Contracts.Responses;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public class GetAllSolutionStacksHandler : IRequestHandler<GetAllSolutionStacks, PaginatedResponse<SolutionStackResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllSolutionStacksHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<SolutionStackResponse>> Handle(
        GetAllSolutionStacks request,
        CancellationToken cancellationToken)
    {
        var query = _context.SolutionStacks.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.Description.ToLower().Contains(searchTerm));
        }

        //TODO: Add more filters as needed
        //if (!string.IsNullOrWhiteSpace(request.Category))
        //{
        //    query = query.Where(s => s.Category == request.Category);
        //}

        //if (!string.IsNullOrWhiteSpace(request.Status))
        //{
        //    query = query.Where(s => s.Status == request.Status);
        //}

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name),
            "createdat" => request.SortDescending
                ? query.OrderByDescending(s => s.CreatedAt)
                : query.OrderBy(s => s.CreatedAt),
            "updatedat" => request.SortDescending
                ? query.OrderByDescending(s => s.UpdatedAt)
                : query.OrderBy(s => s.UpdatedAt),
            _ => query.OrderBy(s => s.Name) // Default sort
        };

        // Apply pagination
        var solutionStacks = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(s => new SolutionStackResponse(
                s.Id,
                s.Name,
                s.Slug,
                s.Description,
                s.Category,
                s.Status,
                s.Owner,
                s.UpdatedAt))
            .ToListAsync(cancellationToken);

        return PaginatedResponse<SolutionStackResponse>.Create(
            solutionStacks,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
