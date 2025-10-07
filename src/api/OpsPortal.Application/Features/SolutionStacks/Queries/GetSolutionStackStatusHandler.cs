using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public class GetSolutionStackStatusHandler : IRequestHandler<GetSolutionStackStatusById, GetSolutionStackStatusResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetSolutionStackStatusHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetSolutionStackStatusResponse?> Handle(GetSolutionStackStatusById request, CancellationToken cancellationToken)
    {
        var solutionStackStatus = await _context.SolutionStackStatuses
            .Where(s => s.Id == request.Id)
            .Select(s => new GetSolutionStackStatusResponse(
                s.Id,
                s.Name,
                s.Slug,
                s.Description))
            .SingleOrDefaultAsync(cancellationToken);

        return solutionStackStatus;
    }
}
