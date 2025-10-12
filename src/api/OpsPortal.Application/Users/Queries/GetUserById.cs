using MediatR;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public record GetUserById(Guid Id) : IRequest<UserResponse?>;
