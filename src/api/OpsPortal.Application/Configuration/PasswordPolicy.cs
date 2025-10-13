namespace OpsPortal.Application.Configuration;

public class PasswordPolicy
{
    public int LockoutDurationMinutes { get; set; } = 15;

    public int MaxAgeDays { get; set; } = 90;

    public int MaxFailedAttempts { get; set; } = 5;

    public int MinimumLength { get; set; } = 12;

    public int PasswordHistoryCount { get; set; } = 5;

    public bool RequireDigit { get; set; } = true;

    public bool RequireLowercase { get; set; } = true;

    public bool RequireSpecialCharacter { get; set; } = true;

    public bool RequireUppercase { get; set; } = true;
}
