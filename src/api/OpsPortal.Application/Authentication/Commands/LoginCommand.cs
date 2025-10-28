using MediatR;
using OpsPortal.Application.Common;
using OpsPortal.Contracts.Authentication;

namespace OpsPortal.Application.Authentication.Commands;

public record LoginCommand : IRequest<OperationResult<LoginResponse>>
{
    public string Identifier { get; init; } = string.Empty;

    public string? IpAddress { get; init; }

    public string Password { get; init; } = string.Empty;

    public string? UserAgent { get; init; }
}
