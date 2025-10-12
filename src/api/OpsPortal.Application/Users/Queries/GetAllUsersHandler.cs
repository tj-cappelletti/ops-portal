using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Queries;

public class GetAllUsersHandler : IRequestHandler<GetAllUsers, PaginatedResponse<UserResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<UserResponse>> Handle(GetAllUsers request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(u => u.DisplayName.ToLower().Contains(searchTerm) ||
                                     (u.Email != null && u.Email.ToLower().Contains(searchTerm)) ||
                                     (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                                     (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)) ||
                                     u.Identifier.ToLower().Contains(searchTerm));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "displayname" => request.SortDescending ? query.OrderByDescending(u => u.DisplayName) : query.OrderBy(u => u.DisplayName),
            "email" => request.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" => request.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => request.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "createdat" => request.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            _ => query.OrderBy(u => u.DisplayName) // Default sorting
        };

        // Apply pagination
        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserResponse(
                u.AvatarUrl,
                u.DisplayName,
                u.Email,
                u.ExternalId,
                u.ExternalMetadata,
                u.FailedLoginAttempts,
                u.FirstName,
                u.Id,
                u.Identifier,
                u.IdentityProvider,
                u.IsDeleted,
                u.IsLocked,
                u.IsSystemUser,
                u.LastLoginAt,
                u.LastLoginIp,
                u.LastName,
                u.Locale,
                u.LockedUntil,
                u.LoginCount,
                u.PasswordChangedAt,
                u.PasswordHash,
                u.Preferences,
                u.RefreshToken,
                u.RefreshTokenExpiry,
                u.RequirePasswordChange,
                u.Status.ToString(),
                u.TimeZone
            ))
            .ToListAsync(cancellationToken);

        return PaginatedResponse<UserResponse>.Create(users, request.PageNumber, request.PageSize, totalCount);
    }
}
