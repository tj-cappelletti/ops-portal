namespace OpsPortal.Application.Configuration;

public class UserIdentificationSettings
{
    public bool AllowUsernameLogin { get; set; } = true;

    public EmailPolicy EmailPolicy { get; set; } = new();

    public UserIdentifierMode Mode { get; set; } = UserIdentifierMode.Flexible;

    public bool RequireEmailForRegistration { get; set; } = false;

    public UsernamePolicy UsernamePolicy { get; set; } = new();
}
