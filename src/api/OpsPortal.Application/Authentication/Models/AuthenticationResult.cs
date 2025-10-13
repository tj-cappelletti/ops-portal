using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Authentication.Models;

public record AuthenticationResult
{
    public string? Error { get; init; }

    public AuthenticationFailureReason? FailureReason { get; init; }

    public bool Succeeded { get; init; }

    public User? User { get; init; }

    public static AuthenticationResult Failed(string error, AuthenticationFailureReason reason = AuthenticationFailureReason.InvalidCredentials)
    {
        return new AuthenticationResult
        {
            Succeeded = false,
            Error = error,
            FailureReason = reason
        };
    }

    public static AuthenticationResult Success(User user)
    {
        return new AuthenticationResult
        {
            Succeeded = true,
            User = user
        };
    }
}
