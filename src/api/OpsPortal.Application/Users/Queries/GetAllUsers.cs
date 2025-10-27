using MediatR;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Queries;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public record GetAllUsers : SearchQuery, IRequest<OperationResult<PaginatedResponse<UserResponse>>>;
