using MediatR;
using OpsPortal.Application.Common.Models;
using OpsPortal.Contracts.Responses;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetAllSolutionStacks : PaginatedRequest, IRequest<PaginatedResponse<SolutionStackResponse>>
{
    public string? SearchTerm { get; init; }
}
