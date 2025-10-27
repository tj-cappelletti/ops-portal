using MediatR;
using Microsoft.EntityFrameworkCore;
using OpsPortal.Application.Common;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Application.Security;
using OpsPortal.Application.Users.Services;
using OpsPortal.Contracts.Users;
using OpsPortal.Domain.Entities;

namespace OpsPortal.Application.Users.Commands;

public class CreateLocalUserCommandHandler : IRequestHandler<CreateLocalUserCommand, OperationResult<UserResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserValidationService _userValidationService;

    public CreateLocalUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IUserValidationService userValidationService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _userValidationService = userValidationService;
    }

    public async Task<OperationResult<UserResponse>> Handle(CreateLocalUserCommand request, CancellationToken cancellationToken)
    {
        if (request.ActionContext == null)
            return OperationError.CreateInternalOperationError("ActionContextMissing", "The action context is missing from the request.");

        var validationResult = await _userValidationService.ValidateUserBusinessRulesAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(businessValidationError => businessValidationError.Code)
                .ToDictionary(
                    grouping => grouping.Key,
                    grouping => grouping.Select(businessValidationError => businessValidationError.Message).ToArray());

            var operationError = OperationError.CreateValidationFailedOperationError(errors);

            return operationError;
        }

        var createdByUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == request.ActionContext.Actor.ActorId, cancellationToken);

        if (createdByUser == null) throw new Exception("Creating user not found");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.CreateLocalUser(
            request.Identifier,
            request.Email,
            request.DisplayName,
            passwordHash,
            false,
            createdByUser);

        await _context.Users.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return user.ToUserResponse();
    }
}
