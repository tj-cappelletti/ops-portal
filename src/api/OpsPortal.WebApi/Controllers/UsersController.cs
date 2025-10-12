using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Users.Queries;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.Users;

namespace OpsPortal.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    public readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
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
}
