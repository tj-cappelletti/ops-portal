using MediatR;
using OpsPortal.Contracts.Responses;

namespace OpsPortal.Application.Features.SolutionStacks.Queries;

public record GetSolutionStackById(Guid Id) : IRequest<SolutionStackResponse?>;
