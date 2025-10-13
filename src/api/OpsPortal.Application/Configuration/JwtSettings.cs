namespace OpsPortal.Application.Configuration;

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

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Secret))
            throw new InvalidOperationException("JWT Secret is required.");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is required.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is required.");

        var validAlgorithms = new[] { "HS256", "HS384", "HS512" };

        if (!validAlgorithms.Contains(Algorithm))
            throw new InvalidOperationException($"Invalid JWT Algorithm. Supported algorithms are: {string.Join(", ", validAlgorithms)}");
    }
}
