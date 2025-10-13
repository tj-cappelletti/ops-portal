namespace OpsPortal.Application.Common.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Algorithm { get; set; } = "HS256";

    public string Audience { get; set; } = string.Empty;

    public int ClockSkewMinutes { get; set; } = 5;

    public int ExpirationMinutes { get; set; } = 60;

    public string Issuer { get; set; } = string.Empty;

    public int RefreshTokenExpirationDays { get; set; } = 7;

    public string Secret { get; set; } = string.Empty;

    public bool ValidateAudience { get; set; } = true;

    public bool ValidateIssuer { get; set; } = true;

    public bool ValidateIssuerSigningKey { get; set; } = true;

    public bool ValidateLifetime { get; set; } = true;
}
