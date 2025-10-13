using OpsPortal.Application.Authentication.Models;

namespace OpsPortal.Application.Authentication.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task RevokeTokenAsync(string username, string revokeReason, CancellationToken cancellationToken = default);
}
