using System.Security.Claims;
using OpsPortal.Application.Authentication.Models;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Authentication.Services;

public interface IJwtService
{
    Task<IEnumerable<Claim>> GenerateClaimsAsync(User user, CancellationToken cancellationToken = default);

    Task<string> GenerateRefreshTokenAsync();

    Task<TokenResult> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);

    DateTime GetTokenExpiration(string token);

    Task<ClaimsPrincipal?> ValidateRefreshTokenAsync(string refreshToken);

    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}
