using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpsPortal.Application.Authentication.Models;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Configuration;
using OpsPortal.Application.Http;
using OpsPortal.Application.Security;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentHttpContext _currentHttpContext;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SecuritySettings _securitySettings;

    public AuthenticationService(
        IApplicationDbContext context,
        ICurrentHttpContext currentHttpContext,
        ILogger<AuthenticationService> logger,
        IPasswordHasher passwordHasher,
        IOptions<SecuritySettings> securitySettings)
    {
        _context = context;
        _currentHttpContext = currentHttpContext;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _securitySettings = securitySettings.Value;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Looking up user with identifier: {Identifier}", identifier);
        var user = _context.Users.SingleOrDefault(u => u.Identifier == identifier);

        if (user == null)
        {
            _logger.LogWarning("User not found with identifier: {Identifier}", identifier);
            return AuthenticationResult.Failed("Invalid credentials");
        }

        // Check if account is locked
        if (user.IsLocked)
        {
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
            {
                _logger.LogWarning("User account '{Identifier}' is locked until {lockedUntil::yyyy-MM-dd HH:mm}", identifier, user.LockedUntil.Value);
                return AuthenticationResult.Failed(
                    $"Account is locked until {user.LockedUntil.Value:yyyy-MM-dd HH:mm} UTC",
                    AuthenticationFailureReason.AccountLocked);
            }

            _logger.LogInformation("Unlocking user account '{Identifier}' as lock period has expired", identifier);
            user.Unlock();
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Check account status
        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("User account '{Identifier}' is {status}", identifier, user.Status.ToString().ToLower());
            return AuthenticationResult.Failed(
                $"Account is {user.Status.ToString().ToLower()}",
                AuthenticationFailureReason.AccountDisabled);
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash!))
        {
            _logger.LogWarning("Invalid password attempt for user '{Identifier}'", identifier);
            await RecordFailedLoginAttemptAsync(user, cancellationToken);
            return AuthenticationResult.Failed("Invalid credentials");
        }

        // Check if password needs rehashing (work factor changed)
        if (_securitySettings.AutoRehashPasswords && _passwordHasher.RequiresRehash(user.PasswordHash!))
        {
            _logger.LogInformation("Rehashing password for user '{Identifier}' due to updated hashing parameters", identifier);
            await RehashPasswordAsync(user, password, cancellationToken);
        }

        // Check if password has expired
        if (_securitySettings.PasswordPolicy.MaxAgeDays > 0)
        {
            var passwordAge = DateTime.UtcNow - (user.PasswordChangedAt ?? user.CreatedAt);

            if (passwordAge.TotalDays > _securitySettings.PasswordPolicy.MaxAgeDays)
            {
                _logger.LogWarning("Password for user '{Identifier}' has expired", identifier);

                user.SetRequirePasswordChange();
                await _context.SaveChangesAsync(cancellationToken);

                return AuthenticationResult.Failed(
                    "Password has expired and must be changed",
                    AuthenticationFailureReason.PasswordExpired);
            }
        }

        // Record successful login
        _logger.LogInformation("Recording successful login for user '{Identifier}'", identifier);
        user.RecordLogin(_currentHttpContext.GetIpAddress());

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User '{Identifier}' authenticated successfully", identifier);

        return AuthenticationResult.Success(user);
    }

    private async Task RecordFailedLoginAttemptAsync(User user, CancellationToken cancellationToken)
    {
        // Lock account after configured number of failed attempts
        var maxAttempts = _securitySettings.PasswordPolicy.MaxFailedAttempts;

        user.RecordFailedLoginAttempt();

        if (maxAttempts > 0 && user.FailedLoginAttempts >= maxAttempts)
        {
            var lockDuration = TimeSpan.FromMinutes(_securitySettings.PasswordPolicy.LockoutDurationMinutes);
            user.LockAccount(lockDuration);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (refreshTokenEntity is not { IsActive: true })
            return AuthenticationResult.Failed("Invalid refresh token");

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == refreshTokenEntity.UserId, cancellationToken);

        return user == null
            ? AuthenticationResult.Failed("User not found")
            : AuthenticationResult.Success(user);
    }

    private async Task RehashPasswordAsync(User user, string password, CancellationToken cancellationToken)
    {
        var newHash = _passwordHasher.HashPassword(password);

        user.UpdatePasswordHash(newHash);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string username, string revokeReason, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Identifier == username, cancellationToken);

        if (user == null) return;

        var userTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.Id && rt.IsActive);

        foreach (var token in userTokens) token.Revoke(revokeReason);
    }
}
