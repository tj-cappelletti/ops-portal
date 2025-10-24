using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Common.Utilities;
using OpsPortal.Application.Configuration;
using OpsPortal.Application.Users.Commands;

namespace OpsPortal.Application.Users.Services;

public class UserValidationService : IUserValidationService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UserValidationService> _logger;

    public UserValidationService(
        IApplicationDbContext context,
        AuthenticationSettings authenticationSettings,
        ILogger<UserValidationService> logger)
    {
        _context = context;
        _authenticationSettings = authenticationSettings;
        _logger = logger;
    }

    private BusinessValidationResult ValidateEmailPolicy(string email)
    {
        if (_authenticationSettings.EmailPolicy.AllowAnyDomain)
            return BusinessValidationResult.Success();

        var errors = new List<BusinessValidationError>();

        var emailDomain = email.Split('@').Last().ToLowerInvariant();
        if (_authenticationSettings.EmailPolicy.BlockedDomains
            .Select(d => d.ToLowerInvariant())
            .Contains(emailDomain))
            errors.Add(new BusinessValidationError(
                "EmailDomainBlocked",
                $"The email domain '{emailDomain}' is blocked by policy."));

        if (_authenticationSettings.EmailPolicy.AllowedDomains
            .Select(d => d.ToLowerInvariant())
            .Contains(emailDomain))
            errors.Add(new BusinessValidationError(
                "EmailDomainNotAllowed",
                $"The email domain '{emailDomain}' is not in the list of allowed domains."));

        return errors.Any()
            ? BusinessValidationResult.Failed(errors.ToArray())
            : BusinessValidationResult.Success();
    }


    private BusinessValidationResult ValidateLocalUserPolicies(CreateLocalUserCommand command)
    {
        var errors = new List<BusinessValidationError>();

        // We only apply these policies for local users
        // If the local authentication is not enabled, we shouldn't be here
        // Let the exception bubble up to catch misconfigurations
        switch (_authenticationSettings.Local!.UserIdentifierMode)
        {
            case UserIdentifierMode.EmailOnly:
                if (command.Identifier != command.Email)
                {
                    errors.Add(new BusinessValidationError(
                        "IdentifierMustBeEmail",
                        "Identifier must match email when system is in email-only mode"));
                }
                else if (string.IsNullOrEmpty(command.Email))
                {
                    errors.Add(new BusinessValidationError(
                        "EmailRequired",
                        "Email is required when system is in email-only mode"));
                }
                else
                {
                    var emailPolicyResult = ValidateEmailPolicy(command.Email);
                    if (!emailPolicyResult.IsValid) errors.AddRange(emailPolicyResult.Errors);
                }

                break;

            case UserIdentifierMode.UsernameOnly:
                if (EmailValidator.IsValid(command.Identifier))
                {
                    errors.Add(new BusinessValidationError(
                        "IdentifierCannotBeEmail",
                        "Identifier cannot be an email address when system is in username-only mode"));
                }
                else
                {
                    // Email is optional in username-only mode
                    if (EmailValidator.IsValid(command.Email))
                    {
                        var emailPolicyResult = ValidateEmailPolicy(command.Email!);
                        if (!emailPolicyResult.IsValid) errors.AddRange(emailPolicyResult.Errors);
                    }

                    var usernamePolicyResult = ValidateUsernamePolicy(command.Identifier);
                    if (!usernamePolicyResult.IsValid) errors.AddRange(usernamePolicyResult.Errors);
                }

                break;
        }

        return errors.Any()
            ? BusinessValidationResult.Failed(errors.ToArray())
            : BusinessValidationResult.Success();
    }

    private Task<BusinessValidationResult> ValidateRoleAssignments(List<string> roleNames)
    {
        throw new NotImplementedException();
        //var existingRoles = await _context.Roles
        //    .Where(r => roleNames.Contains(r.Name) && r.IsActive)
        //    .Select(r => r.Name)
        //    .ToListAsync();

        //var missingRoles = roleNames.Except(existingRoles).ToList();

        //if (missingRoles.Any())
        //{
        //    return BusinessValidationResult.Failed(
        //        new BusinessValidationError(
        //            "InvalidRoles",
        //            $"The following roles do not exist or are inactive: {string.Join(", ", missingRoles)}"));
        //}

        //return BusinessValidationResult.Success();
    }

    public async Task<BusinessValidationResult> ValidateUniqueUserAsync(string identifier, string? email)
    {
        var errors = new List<BusinessValidationError>();

        var userCount = await _context.Users
            .Where(u => u.Identifier == identifier.ToLowerInvariant() ||
                        (!string.IsNullOrEmpty(email) && u.Email == email.ToLowerInvariant()))
            .CountAsync();

        if (userCount > 0)
            errors.Add(new BusinessValidationError(
                "UserAlreadyExists",
                $"A user with identifier '{identifier}' and/or email '{email}' already exists"));

        return errors.Any()
            ? BusinessValidationResult.Failed(errors.ToArray())
            : BusinessValidationResult.Success();
    }

    public async Task<BusinessValidationResult> ValidateUserBusinessRulesAsync(CreateLocalUserCommand command)
    {
        var errors = new List<BusinessValidationError>();

        var uniquenessResult = await ValidateUniqueUserAsync(command.Identifier, command.Email);
        if (!uniquenessResult.IsValid) errors.AddRange(uniquenessResult.Errors);

        switch (_authenticationSettings.Mode)
        {
            case AuthenticationMode.EntraID:
                throw new NotImplementedException();

            case AuthenticationMode.Local:
                var localUserResult = ValidateLocalUserPolicies(command);
                if (!localUserResult.IsValid) errors.AddRange(localUserResult.Errors);
                break;

            default:
                _logger.LogError("Unknown authentication mode: {Mode}", _authenticationSettings.Mode);
                throw new InvalidOperationException($"Unknown authentication mode: {_authenticationSettings.Mode}");
        }

        return errors.Any()
            ? BusinessValidationResult.Failed(errors.ToArray())
            : BusinessValidationResult.Success();
    }

    private BusinessValidationResult ValidateUsernamePolicy(string username)
    {
        // We only apply these policies for local users
        // If the local authentication is not enabled, we shouldn't be here
        // Let the exception bubble up to catch misconfigurations
        if (username.Length < _authenticationSettings.Local!.UsernamePolicy.MinLength ||
            username.Length > _authenticationSettings.Local.UsernamePolicy.MaxLength)
            return BusinessValidationResult.Failed(
                new BusinessValidationError(
                    "UsernameLengthInvalid",
                    $"Username must be between {_authenticationSettings.Local.UsernamePolicy.MinLength} and {_authenticationSettings.Local.UsernamePolicy.MaxLength} characters long."));

        if (!_authenticationSettings.Local.UsernamePolicy.AllowNumericOnly && username.All(char.IsDigit))
            return BusinessValidationResult.Failed(
                new BusinessValidationError(
                    "UsernameNumericOnly",
                    "Username cannot be numeric only."));

        if (!string.IsNullOrEmpty(_authenticationSettings.Local.UsernamePolicy.Pattern))
        {
            var regex = new Regex(_authenticationSettings.Local.UsernamePolicy.Pattern);
            if (!regex.IsMatch(username))
                return BusinessValidationResult.Failed(
                    new BusinessValidationError(
                        "UsernamePatternInvalid",
                        $"Username does not match the required pattern: {_authenticationSettings.Local.UsernamePolicy.PatternDescription}."));
        }

        if (_authenticationSettings.Local.UsernamePolicy.ReservedUsernames
            .Select(u => u.ToLowerInvariant())
            .Contains(username.ToLowerInvariant()))
            return BusinessValidationResult.Failed(
                new BusinessValidationError(
                    "UsernameReserved",
                    "The chosen username is reserved and cannot be used."));

        return BusinessValidationResult.Success();
    }
}
