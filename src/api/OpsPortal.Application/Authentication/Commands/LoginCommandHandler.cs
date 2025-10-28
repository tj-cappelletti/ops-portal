using MediatR;
using Microsoft.Extensions.Logging;
using OpsPortal.Application.Authentication.Models;
using OpsPortal.Application.Authentication.Services;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Authentication;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Authentication.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, OperationResult<LoginResponse>>
{
    private readonly IAuthenticationService _authService;
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IAuthenticationService authService, IApplicationDbContext context, IJwtService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _authService = authService;
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<OperationResult<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to authenticate user with identifier: {Identifier}", request.Identifier);
        var authResult = await _authService.AuthenticateAsync(
            request.Identifier,
            request.Password,
            cancellationToken);

        if (!authResult.Succeeded)
        {
            _logger.LogWarning("Authentication failed for identifier: {Identifier}, Reason: {Reason}", request.Identifier, authResult.FailureReason);

            var message = authResult.FailureReason switch
            {
                AuthenticationFailureReason.AccountLocked => "Account is locked",
                AuthenticationFailureReason.AccountDisabled => "Account is disabled",
                AuthenticationFailureReason.PasswordExpired => "Password change required",
                AuthenticationFailureReason.RequiresTwoFactor => "Two-factor authentication required",
                AuthenticationFailureReason.EmailNotVerified => "Email address is not verified",
                _ => "Invalid credentials"
            };

            return OperationError.CreateUnauthorizedOperationError(message);
        }

        if (authResult.User == null)
        {
            _logger.LogError("Authentication succeeded but no user was returned for identifier: {Identifier}", request.Identifier);
            return OperationError.CreateInternalOperationError("AuthenticationFailed", "Authentication succeeded but no user was returned.");
        }

        _logger.LogInformation("User authenticated successfully: {UserId}", authResult.User.Id);
        _logger.LogInformation("Generating JWT token for user: {UserId}", authResult.User.Id);
        var tokenResult = await _jwtService.GenerateTokenAsync(authResult.User, cancellationToken);

        _logger.LogInformation("Storing refresh token for user: {UserId}", authResult.User.Id);
        var refreshToken = RefreshToken.Create(
            authResult.User.Id,
            tokenResult.RefreshToken,
            tokenResult.ExpiresAt,
            request.UserAgent,
            request.IpAddress);

        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login process completed successfully for user: {UserId}", authResult.User.Id);

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
