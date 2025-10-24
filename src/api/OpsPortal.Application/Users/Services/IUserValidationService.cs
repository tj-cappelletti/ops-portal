using OpsPortal.Application.Common;
using OpsPortal.Application.Users.Commands;

namespace OpsPortal.Application.Users.Services;

public interface IUserValidationService
{
    Task<BusinessValidationResult> ValidateUserBusinessRulesAsync(CreateLocalUserCommand command);

    Task<BusinessValidationResult> ValidateUniqueUserAsync(string identifier, string? email);
}
