using MediatR;
using OpsPortal.Contracts.SolutionStacks;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetSolutionStackById(Guid Id) : IRequest<GetSolutionStackResponse?>;
