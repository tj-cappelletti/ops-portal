namespace OpsPortal.Application.Configuration;

public class LocalAuthSettings
{
    public UserIdentifierMode UserIdentifierMode { get; set; } = UserIdentifierMode.Flexible;

    public UsernamePolicy UsernamePolicy { get; set; } = new();
}
