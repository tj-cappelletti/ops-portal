using System.Text.RegularExpressions;

namespace OpsPortal.Application.Common.Utilities;

public static class EmailValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

    public static bool IsValid(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }
}
