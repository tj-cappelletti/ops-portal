using MediatR;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetSolutionStackStatusById(Guid Id) : IRequest<GetSolutionStackStatusResponse?>;
