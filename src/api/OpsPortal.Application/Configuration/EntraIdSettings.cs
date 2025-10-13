namespace OpsPortal.Application.Configuration;

public class EntraIdSettings
{
    public string CallbackPath { get; set; } = "/signin-oidc";
    
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    
    public string Domain { get; set; } = string.Empty;
    
    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    
    public string TenantId { get; set; } = string.Empty;
}
