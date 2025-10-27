using MediatR;
using OpsPortal.Application.Common;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public record GetUserById(Guid Id) : IRequest<OperationResult<UserResponse>>;
