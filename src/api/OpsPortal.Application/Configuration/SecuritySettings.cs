namespace OpsPortal.Application.Configuration;

public class SecuritySettings
{
    /// <summary>
    ///     Automatically rehash passwords with old work factor on successful login
    /// </summary>
    public bool AutoRehashPasswords { get; set; } = true;

    /// <summary>
    ///     BCrypt work factor (cost factor). Higher = more secure but slower.
    ///     Recommended: 10-13 for 2024+ hardware
    /// </summary>
    public int BCryptWorkFactor { get; set; } = 12;

    /// <summary>
    ///     Password policy settings
    /// </summary>
    public PasswordPolicy PasswordPolicy { get; set; } = new();
}
