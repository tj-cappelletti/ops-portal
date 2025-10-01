using MediatR;
using OpsPortal.Application.Common.Models;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetAllSolutionStacks : PaginatedRequest, IRequest<PaginatedResponse<GetSolutionStackResponse>>
{
    public string? SearchTerm { get; init; }
}
