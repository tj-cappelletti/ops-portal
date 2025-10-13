namespace OpsPortal.Application.Configuration;

public class LocalAuthSettings
{
    public string JwtSecret { get; set; } = string.Empty;

    public PasswordPolicy PasswordPolicy { get; set; } = new();

    public int RefreshTokenExpirationDays { get; set; } = 7;

    public int TokenExpirationMinutes { get; set; } = 60;
}
