namespace OpsPortal.Application.Configuration;

public class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    public EntraIdSettings? EntraId { get; set; }

    public JwtSettings Jwt { get; set; } = new JwtSettings();

    public LocalAuthSettings? Local { get; set; }
    
    public AuthenticationMode Mode { get; set; } = AuthenticationMode.Local;

    public void Validate()
    {
        switch (Mode)
        {
            case AuthenticationMode.Local:
                if (Local == null)
                    throw new InvalidOperationException("Local authentication settings are required when Mode is Local");
                break;
            
            case AuthenticationMode.AzureAd:
                if (EntraId == null)
                    throw new InvalidOperationException("Azure AD settings are required when Mode is AzureAd");

                EntraId.Validate();
                break;
            
            default:
                throw new InvalidOperationException($"Unknown authentication mode: {Mode}");
        }

        Jwt.Validate();
    }
}
