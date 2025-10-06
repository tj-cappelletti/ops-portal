using MediatR;
using OpsPortal.Application.Common.Queries;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetAllSolutionStacks : SearchQuery, IRequest<PaginatedResponse<GetSolutionStackResponse>>;
