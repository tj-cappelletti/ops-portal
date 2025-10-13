namespace OpsPortal.Application.Authentication.Models;

public enum AuthenticationFailureReason
{
    InvalidCredentials,
    AccountLocked,
    AccountDisabled,
    PasswordExpired,
    RequiresTwoFactor,
    EmailNotVerified
}
