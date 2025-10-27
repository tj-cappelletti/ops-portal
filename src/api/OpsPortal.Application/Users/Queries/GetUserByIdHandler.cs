using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public class GetUserByIdHandler : IRequestHandler<GetUserById, OperationResult<UserResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<UserResponse>> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        return user == null
            ? OperationError.CreateNotFoundOperationError("User", request.Id)
            : user.ToUserResponse();
    }
}
