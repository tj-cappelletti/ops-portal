using MediatR;
using OpsPortal.Contracts.Responses;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetSolutionStack(Guid Id) : IRequest<SolutionStackResponse?>;
