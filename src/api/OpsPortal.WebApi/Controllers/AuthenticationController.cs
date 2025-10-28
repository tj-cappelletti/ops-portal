using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Authentication.Commands;
using OpsPortal.Application.Configuration;
using OpsPortal.Application.Http;
using OpsPortal.Contracts.Authentication;
using OpsPortal.WebApi.Extensions;

namespace OpsPortal.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ApiControllerBase<AuthenticationController>
{
    private const string OperationName = nameof(AuthenticationController);

    private readonly AuthenticationSettings _authSettings;
    private readonly ICurrentHttpContext _currentHttpContext;
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator, AuthenticationSettings authSettings, ICurrentHttpContext currentHttpContext,
        ILogger<AuthenticationController> logger) : base(logger)
    {
        _mediator = mediator;
        _authSettings = authSettings;
        _currentHttpContext = currentHttpContext;
    }

    [HttpGet("mode")]
    [AllowAnonymous]
    public IActionResult GetAuthenticationMode()
    {
        var loggerState = new Dictionary<string, object>
        {
            ["Operation"] = $"{OperationName}-{nameof(GetAuthenticationMode)}",
            ["RequestId"] = HttpContext.TraceIdentifier,
            ["CorrelationId"] = HttpContext.GetCorrelationId(),
            ["ClientIp"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
        };

        using (Logger.BeginScope(loggerState))
        {
            Logger.LogDebug("Retrieving authentication mode");

            var mode = _authSettings.Mode;
            var ssoEnabled = mode != AuthenticationMode.Local;
            var ssoLoginUrl = mode == AuthenticationMode.EntraID ? "/signin-oidc" : null;

            Logger.LogInformation("Getting authentication mode: {mode}", _authSettings.Mode);
            Logger.LogDebug("AuthenticationModeResponse - Mode: {mode}, SsoEnabled: {ssoEnabled}, SsoLoginUrl: {ssoLoginUrl}", mode, ssoEnabled, ssoLoginUrl);

            return Ok(new AuthenticationModeResponse
            {
                Mode = mode.ToString(),
                SsoEnabled = ssoEnabled,
                SsoLoginUrl = ssoLoginUrl
            });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody]LoginRequest request)
    {
        var loggerState = new Dictionary<string, object>
        {
            ["Operation"] = $"{OperationName}-{nameof(GetAuthenticationMode)}",
            ["Identifier"] = request.Identifier,
            ["RequestId"] = HttpContext.TraceIdentifier,
            ["CorrelationId"] = HttpContext.GetCorrelationId(),
            ["ClientIp"] = _currentHttpContext.GetIpAddress() ?? "Unknown"
        };

        using (Logger.BeginScope(loggerState))
        {
            Logger.LogDebug("Processing login request for Identifier: {Identifier}", request.Identifier);

            if (_authSettings.Mode != AuthenticationMode.Local)
            {
                Logger.LogWarning("Local authentication is not enabled. Current mode: {Mode}", _authSettings.Mode);
                return BadRequest(new { error = "Local authentication is not enabled" });
            }

            var command = new LoginCommand
            {
                Identifier = request.Identifier,
                IpAddress = _currentHttpContext.GetIpAddress(),
                Password = request.Password,
                UserAgent = _currentHttpContext.GetUserAgent()
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                Logger.LogWarning("Login failed for Identifier: {Identifier} with error: {Error}", request.Identifier, result.Error?.Message);
                return HandleOperationError(result.Error);
            }

            Logger.LogInformation("Login successful for Identifier: {Identifier}", request.Identifier);
            return HandleOperationResult(result);
        }
    }

    //[HttpPost("refresh")]
    //[AllowAnonymous]
    //public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request)
    //{
    //    if (_authSettings.Mode != AuthenticationMode.Local) return BadRequest(new { error = "Local authentication is not enabled" });

    //    var command = new RefreshTokenCommand(request.RefreshToken);
    //    var result = await _mediator.Send(command);

    //    if (!result.Succeeded) return Unauthorized(new { error = result.Error });

    //    return Ok(new
    //    {
    //        token = result.Token,
    //        refreshToken = result.RefreshToken,
    //        expiresAt = result.ExpiresAt
    //    });
    //}
}
