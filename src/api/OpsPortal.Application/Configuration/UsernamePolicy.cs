namespace OpsPortal.Application.Configuration;

public class UsernamePolicy
{
    public bool AllowNumericOnly { get; set; } = false;

    public int MaxLength { get; set; } = 50;

    public int MinLength { get; set; } = 3;

    public string Pattern { get; set; } = @"^[a-zA-Z0-9_-]+$";

    public string PatternDescription { get; set; } = "Letters, numbers, dash, and underscore only";

    public List<string> ReservedUsernames { get; set; } =
    [
        "admin",
        "api",
        "administrator",
        "bot",
        "demo",
        "operator",
        "root",
        "service",
        "system",
        "test",
        "user"
    ];
}
