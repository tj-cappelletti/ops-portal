namespace OpsPortal.Application.Configuration;

public class LocalAuthSettings
{
    public PasswordPolicy PasswordPolicy { get; set; } = new();

    public UserIdentificationSettings UserIdentification { get; set; } = new();
}
