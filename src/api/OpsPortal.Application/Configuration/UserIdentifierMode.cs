namespace OpsPortal.Application.Configuration;

public enum UserIdentifierMode
{
    Flexible, // Allow username OR email (default)
    EmailOnly, // Enterprise and SSO: Require email addresses
    UsernameOnly // No email at all
}
