using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public class GetUserByIdHandler : IRequestHandler<GetUserById, UserResponse?>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponse?> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null) return null;

        return new UserResponse(
            user.AvatarUrl,
            user.DisplayName,
            user.Email,
            user.ExternalId,
            user.ExternalMetadata,
            user.FailedLoginAttempts,
            user.FirstName,
            user.Id,
            user.Identifier,
            user.IdentityProvider,
            user.IsDeleted,
            user.IsLocked,
            user.IsSystemUser,
            user.LastLoginAt,
            user.LastLoginIp,
            user.LastName,
            user.Locale,
            user.LockedUntil,
            user.LoginCount,
            user.PasswordChangedAt,
            user.Preferences,
            user.RefreshTokenExpiry,
            user.RequirePasswordChange,
            user.Status.ToString(),
            user.TimeZone
        );
    }
}
