namespace OpsPortal.Application.Authentication.Models;

public record TokenResult(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt);
