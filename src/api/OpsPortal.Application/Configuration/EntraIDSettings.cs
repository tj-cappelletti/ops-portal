namespace OpsPortal.Application.Configuration;

// Entra ID is the proper name, this still meets C# naming conventions
// ReSharper disable once InconsistentNaming
public class EntraIDSettings
{
    public string CallbackPath { get; set; } = "/signin-oidc";
    
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    
    public string Domain { get; set; } = string.Empty;
    
    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    
    public string TenantId { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new InvalidOperationException("Entra ID ClientId is required.");

        if (string.IsNullOrWhiteSpace(ClientSecret))
            throw new InvalidOperationException("Entra ID ClientSecret is required.");

        if (string.IsNullOrWhiteSpace(Domain))
            throw new InvalidOperationException("Entra ID Domain is required.");

        if (string.IsNullOrWhiteSpace(TenantId))
            throw new InvalidOperationException("Entra ID TenantId is required.");
    }
}
