using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpsPortal.Application.Authentication.Models;
using OpsPortal.Application.Authentication.Services;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Configuration;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Infrastructure.Authentication.Services;

public class JwtService : IJwtService
{
    private readonly IApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtService(
        AuthenticationSettings authenticationSettings,
        IApplicationDbContext context,
        ILogger<JwtService> logger)
    {
        _jwtSettings = authenticationSettings.Jwt;
        _context = context;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public Task<IEnumerable<Claim>> GenerateClaimsAsync(User user, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new("sub", user.Id.ToString()),
            new("identifier", user.Identifier),
            // TODO: Consider if we need the auth type since we only support one mode at a time
            //new("auth_type", user.AuthType.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("email", user.Email));
        }

        // Add optional claims
        if (!string.IsNullOrEmpty(user.FirstName))
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));

        if (!string.IsNullOrEmpty(user.LastName))
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

        if (!string.IsNullOrEmpty(user.AvatarUrl))
            claims.Add(new Claim("avatar_url", user.AvatarUrl));

        // Add role claims
        //var userRoles = await _context.UserRoles
        //    .Include(ur => ur.Role)
        //    .Where(ur => ur.UserId == user.Id && ur.Role.IsActive)
        //    .Select(ur => ur.Role.Name)
        //    .ToListAsync(cancellationToken);

        //foreach (var role in userRoles)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, role));
        //}

        // Add team claims
        //var userTeams = await _context.TeamMembers
        //    .Include(tm => tm.Team)
        //    .Where(tm => tm.UserId == user.Id && tm.IsActive)
        //    .Select(tm => tm.Team.Name)
        //    .ToListAsync(cancellationToken);

        //foreach (var team in userTeams)
        //{
        //    claims.Add(new Claim("team", team));
        //}

        // Add permission claims (if using claims-based authorization)
        //var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);
        //foreach (var permission in permissions)
        //{
        //    claims.Add(new Claim("permission", permission));
        //}

        //_logger.LogDebug("Generated {ClaimCount} claims for user {UserId}",
        //    claims.Count, user.Id);

        return Task.FromResult(claims.AsEnumerable());
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = Convert.ToBase64String(randomBytes);

        _logger.LogDebug("Generated refresh token");

        return await Task.FromResult(refreshToken);
    }

    public async Task<TokenResult> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating claims for user {UserId} ({Email})", user.Id, user.Email);
        var claims = await GenerateClaimsAsync(user, cancellationToken);

        _logger.LogInformation("Loading JWT secret and generating key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow
        };

        _logger.LogInformation("Creating JWT token for user {UserId} ({Email})", user.Id, user.Email);
        var token = _tokenHandler.CreateToken(tokenDescriptor);

        _logger.LogInformation("Writing JWT token to string for user {UserId} ({Email})", user.Id, user.Email);
        var tokenString = _tokenHandler.WriteToken(token);

        _logger.LogDebug("Generated JWT token for user {UserId} ({Email}), expires at {Expiration}",
            user.Id, user.Email, tokenDescriptor.Expires);

        return new TokenResult(
            tokenString,
            await GenerateRefreshTokenAsync(),
            tokenDescriptor.Expires.Value);
    }

    public DateTime GetTokenExpiration(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read token expiration");
            return DateTime.MinValue;
        }
    }

    //private async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    //{
    //    // Get permissions from roles
    //    var rolePermissions = await _context.UserRoles
    //        .Where(ur => ur.UserId == userId)
    //        .SelectMany(ur => ur.Role.RolePermissions)
    //        .Select(rp => rp.Permission.ToPermissionString())
    //        .ToListAsync(cancellationToken);

    //    // Get direct user permissions
    //    var userPermissions = await _context.UserPermissions
    //        .Where(up => up.UserId == userId &&
    //                    (up.ExpiresAt == null || up.ExpiresAt > DateTime.UtcNow))
    //        .Select(up => up.Permission.ToPermissionString())
    //        .ToListAsync(cancellationToken);

    //    // Get team permissions
    //    var teamPermissions = await _context.TeamMembers
    //        .Where(tm => tm.UserId == userId && tm.IsActive)
    //        .SelectMany(tm => tm.Team.TeamPermissions)
    //        .Select(tp => tp.Permission.ToPermissionString())
    //        .ToListAsync(cancellationToken);

    //    return rolePermissions
    //        .Concat(userPermissions)
    //        .Concat(teamPermissions)
    //        .Distinct();
    //}

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromMinutes(_jwtSettings.ClockSkewMinutes),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
            ValidateLifetime = _jwtSettings.ValidateLifetime,
            ValidAudience = _jwtSettings.Audience,
            ValidIssuer = _jwtSettings.Issuer
        };
    }

    public async Task<ClaimsPrincipal?> ValidateRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var storedRefreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken &&
                                           rt.ExpiresAt > DateTime.UtcNow &&
                                           !rt.IsRevoked);

            if (storedRefreshToken == null)
            {
                _logger.LogWarning("Invalid or expired refresh token");
                return null;
            }

            var claims = await GenerateClaimsAsync(storedRefreshToken.User);
            var identity = new ClaimsIdentity(claims, "refresh");
            var principal = new ClaimsPrincipal(identity);

            _logger.LogDebug("Successfully validated refresh token for user {UserId}",
                storedRefreshToken.User.Id);

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token");
            return null;
        }
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenValidationParameters = GetTokenValidationParameters();

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                _logger.LogWarning("Invalid token format");
                return null;
            }

            // Ensure it's a JWT token with the correct algorithm
            if (!jwtToken.Header.Alg.Equals(_jwtSettings.Algorithm, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Invalid token algorithm. Expected {Expected}, got {Actual}",
                    _jwtSettings.Algorithm, jwtToken.Header.Alg);
                return null;
            }

            _logger.LogDebug("Successfully validated JWT token for user {UserId}",
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return await Task.FromResult(principal);
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogDebug("Token expired: {Message}", ex.Message);
            return null;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning("Token validation failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token validation");
            return null;
        }
    }
}
