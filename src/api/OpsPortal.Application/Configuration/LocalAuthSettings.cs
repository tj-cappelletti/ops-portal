namespace OpsPortal.Application.Configuration;

public class LocalAuthSettings
{
    public PasswordPolicy PasswordPolicy { get; set; } = new();
}
