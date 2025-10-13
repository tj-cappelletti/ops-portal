using BCrypt.Net;
using Microsoft.Extensions.Options;
using OpsPortal.Application.Configuration;
using OpsPortal.Application.Security;

namespace OpsPortal.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    private readonly SecuritySettings _settings;

    public BCryptPasswordHasher(IOptions<SecuritySettings> settings)
    {
        _settings = settings.Value;
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        // Work factor from configuration (default 12 for good security/performance balance)
        var workFactor = _settings.BCryptWorkFactor;

        // Use EnhancedHashPassword for better security (uses SHA384 internally for long passwords)
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            // EnhancedVerify handles both standard and enhanced hashes
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        }
        catch (SaltParseException)
        {
            // Invalid hash format
            return false;
        }
    }

    public bool RequiresRehash(string hash)
    {
        try
        {
            // Check if the hash needs to be upgraded (work factor changed or using old format)
            var hashWorkFactor = BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, _settings.BCryptWorkFactor);

            return hashWorkFactor;
        }
        catch
        {
            // If we can't parse the hash, it probably needs rehashing
            return true;
        }
    }
}
