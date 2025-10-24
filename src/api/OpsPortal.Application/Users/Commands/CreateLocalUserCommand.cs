using MediatR;
using OpsPortal.Application.Auditing;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Contracts.Users;

namespace OpsPortal.Application.Users.Commands;

public record CreateLocalUserCommand : IActionContextAware, IRequest<OperationResult<UserResponse>>
{
    public ActionContext? ActionContext { get; set; }

    public string? AvatarUrl { get; init; }

    public string DisplayName { get; init; }

    public string? Email { get; init; }

    public string? FirstName { get; init; }

    public string Identifier { get; init; }

    public string? LastName { get; init; }

    public string? Locale { get; init; }

    public string Password { get; init; }

    public bool? RequirePasswordChange { get; init; }

    public string? TimeZone { get; init; }

    public CreateLocalUserCommand(
        string? avatarUrl,
        string displayName,
        string? email,
        string? firstName,
        string identifier,
        string? lastName,
        string? locale,
        string password,
        bool? requirePasswordChange,
        string? timeZone)
    {
        AvatarUrl = avatarUrl;
        DisplayName = displayName;
        Email = email;
        FirstName = firstName;
        Identifier = identifier;
        LastName = lastName;
        Locale = locale;
        Password = password;
        RequirePasswordChange = requirePasswordChange;
        TimeZone = timeZone;
    }
}
