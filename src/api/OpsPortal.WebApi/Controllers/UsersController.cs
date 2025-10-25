using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Users.Commands;
using OpsPortal.Application.Users.Queries;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.Users;

namespace OpsPortal.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase<UsersController>
{
    private readonly IMediator _mediator;

    public UsersController(ILogger<UsersController> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    [HttpPost("local")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateLocalUser([FromBody]CreateLocalUserRequest request)
    {
        var loggerState = new Dictionary<string, object>
        {
            ["Operation"] = nameof(CreateLocalUser),
            ["Email"] = request.Email ?? string.Empty,
            ["RequestId"] = HttpContext.TraceIdentifier,
            ["CorrelationId"] = CorrelationId ?? HttpContext.TraceIdentifier,
            ["ClientIp"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
        };

        using (Logger.BeginScope(loggerState))
        {
            Logger.LogDebug("Creating CreateLocalUserCommand");
            var command = new CreateLocalUserCommand(
                request.AvatarUrl,
                request.DisplayName,
                request.Email,
                request.FirstName,
                request.Identifier,
                request.LastName,
                request.Locale,
                request.Password,
                request.RequirePasswordChange,
                request.TimeZone);

            Logger.LogDebug(
                "Created CreateLocalUserCommand with Identifier '{Identifier}' and email '{Email}'",
                request.Identifier,
                request.Email);

            Logger.LogInformation("Sending CreateLocalUserCommand to mediator");
            var operationResult = await _mediator.Send(command);

            Logger.LogInformation(
                "CreateLocalUserCommand completed with success: {IsSuccess}",
                operationResult.IsSuccess);

            // The operationResult.value will be non-null if IsSuccess is true
            // If it is null, let it throw to be caught by the global exception handler
            return operationResult.IsSuccess
                ? CreatedAtAction(nameof(GetUserById), new { id = operationResult.Value!.Id }, operationResult.Value)
                : HandleOperationError(operationResult.Error);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery]GetAllUsersRequest request)
    {
        var query = new GetAllUsers
        {
            SearchTerm = request.SearchTerm,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var query = new GetUserById(id);

        var user = await _mediator.Send(query);

        if (user == null) return NotFound();

        return Ok(user);
    }
}
