namespace OpsPortal.Application.Configuration;

public class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    // Entra ID is the proper name, this still meets C# naming conventions
    // ReSharper disable once InconsistentNaming
    public EntraIDSettings? EntraID { get; set; }

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
                if (EntraID == null)
                    throw new InvalidOperationException("Azure AD settings are required when Mode is AzureAd");

                EntraID.Validate();
                break;
            
            default:
                throw new InvalidOperationException($"Unknown authentication mode: {Mode}");
        }

        Jwt.Validate();
    }
}
