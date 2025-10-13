using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Authentication.Commands;
using OpsPortal.Application.Configuration;
using OpsPortal.Application.Http;
using OpsPortal.Contracts.Authentication;

namespace OpsPortal.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationSettings _authSettings;
    private readonly ICurrentHttpContext _currentHttpContext;
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator, AuthenticationSettings authSettings, ICurrentHttpContext currentHttpContext)
    {
        _mediator = mediator;
        _authSettings = authSettings;
        _currentHttpContext = currentHttpContext;
    }

    [HttpGet("mode")]
    [AllowAnonymous]
    public IActionResult GetAuthenticationMode()
    {
        return Ok(new AuthenticationModeResponse
        {
            Mode = _authSettings.Mode.ToString(),
            SsoEnabled = _authSettings.Mode != AuthenticationMode.Local,
            SsoLoginUrl = _authSettings.Mode == AuthenticationMode.AzureAd ? "/signin-oidc" : null
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody]LoginRequest request)
    {
        if (_authSettings.Mode != AuthenticationMode.Local) return BadRequest(new { error = "Local authentication is not enabled" });

        var command = new LoginCommand
        {
            Identifier = request.Identifier,
            IpAddress = _currentHttpContext.GetIpAddress(),
            Password = request.Password
        };

        var result = await _mediator.Send(command);

        if (!result.Succeeded) return Unauthorized(new { error = result.Message });

        return Ok(new
        {
            token = result.Token,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt,
            user = result.User
        });
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
