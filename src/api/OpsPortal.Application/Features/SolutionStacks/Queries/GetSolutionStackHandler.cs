using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public class GetSolutionStackHandler : IRequestHandler<GetSolutionStackById, GetSolutionStackResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetSolutionStackHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetSolutionStackResponse?> Handle(GetSolutionStackById request, CancellationToken cancellationToken)
    {
        var solutionStack = await _context.SolutionStacks
            .Where(s => s.Id == request.Id)
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
            .SingleOrDefaultAsync(cancellationToken);

        return solutionStack;
    }
}
