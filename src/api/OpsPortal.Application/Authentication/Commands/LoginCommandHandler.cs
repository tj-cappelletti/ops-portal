using MediatR;
using OpsPortal.Application.Authentication.Services;
using OpsPortal.Contracts.Authentication;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Authentication.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthenticationService _authService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IAuthenticationService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Use internal AuthenticationResult
        var authResult = await _authService.AuthenticateAsync(
            request.Identifier,
            request.Password,
            cancellationToken);

        if (!authResult.Succeeded)
            //throw new UnauthorizedException(authResult.Error);
            throw new Exception(authResult.Error);

        // Convert to external contract
        var tokenResult = await _jwtService.GenerateTokenAsync(authResult.User!, cancellationToken);

        return new LoginResponse
        {
            Token = tokenResult.Token,
            RefreshToken = tokenResult.RefreshToken,
            ExpiresAt = tokenResult.ExpiresAt,
            User = MapToUserInfo(authResult.User!) // Map domain to contract
        };
    }

    private static UserInfo MapToUserInfo(User user)
    {
        return new UserInfo
        {
            Id = user.Id,
            Identifier = user.Identifier,
            DisplayName = user.DisplayName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl
            // Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
