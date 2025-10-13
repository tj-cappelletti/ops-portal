using Microsoft.EntityFrameworkCore;
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
    private readonly IPasswordHasher _passwordHasher;
    private readonly SecuritySettings _securitySettings;

    public AuthenticationService(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IOptions<SecuritySettings> securitySettings,
        ICurrentHttpContext currentHttpContext)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _securitySettings = securitySettings.Value;
        _currentHttpContext = currentHttpContext;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string identifier, string password, CancellationToken cancellationToken)
    {
        var user = _context.Users.SingleOrDefault(u => u.Identifier == identifier);

        if (user == null) return AuthenticationResult.Failed("Invalid credentials");

        // Check if account is locked
        if (user.IsLocked)
        {
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                return AuthenticationResult.Failed(
                    $"Account is locked until {user.LockedUntil.Value:yyyy-MM-dd HH:mm} UTC",
                    AuthenticationFailureReason.AccountLocked);

            // Unlock if lock period has expired
            user.Unlock();
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Check account status
        if (user.Status != UserStatus.Active)
            return AuthenticationResult.Failed(
                $"Account is {user.Status.ToString().ToLower()}",
                AuthenticationFailureReason.AccountDisabled);

        // Verify password
        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash!))
        {
            await RecordFailedLoginAttemptAsync(user, cancellationToken);
            return AuthenticationResult.Failed("Invalid credentials");
        }

        // Check if password needs rehashing (work factor changed)
        if (_securitySettings.AutoRehashPasswords &&
            _passwordHasher.RequiresRehash(user.PasswordHash!))
            await RehashPasswordAsync(user, password, cancellationToken);

        // Check if password has expired
        if (_securitySettings.PasswordPolicy.MaxAgeDays > 0)
        {
            var passwordAge = DateTime.UtcNow - (user.PasswordChangedAt ?? user.CreatedAt);
            if (passwordAge.TotalDays > _securitySettings.PasswordPolicy.MaxAgeDays)
            {
                user.SetRequirePasswordChange();
                await _context.SaveChangesAsync(cancellationToken);

                return AuthenticationResult.Failed(
                    "Password has expired and must be changed",
                    AuthenticationFailureReason.PasswordExpired);
            }
        }

        // Record successful login
        user.RecordLogin(_currentHttpContext.GetIpAddress());

        await _context.SaveChangesAsync(cancellationToken);

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
