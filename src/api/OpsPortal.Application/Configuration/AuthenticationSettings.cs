namespace OpsPortal.Application.Configuration;

public class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    public EmailPolicy EmailPolicy { get; set; } = new();

    // Entra ID is the proper name, this still meets C# naming conventions
    // ReSharper disable once InconsistentNaming
    public EntraIDSettings? EntraID { get; set; }

    public JwtSettings Jwt { get; set; } = new();

    public LocalAuthSettings? Local { get; set; }

    public AuthenticationMode Mode { get; set; } = AuthenticationMode.Local;

    public void Validate()
    {
        switch (Mode)
        {
            case AuthenticationMode.EntraID:
                if (EntraID == null)
                    throw new InvalidOperationException("Azure AD settings are required when Mode is Entra ID");

                EntraID.Validate();
                break;

            case AuthenticationMode.Local:
                if (Local == null)
                    throw new InvalidOperationException("Local authentication settings are required when Mode is Local");
                break;

            default:
                throw new InvalidOperationException($"Unknown authentication mode: {Mode}");
        }

        Jwt.Validate();
    }
}
