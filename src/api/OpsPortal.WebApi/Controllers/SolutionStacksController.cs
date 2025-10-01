using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Features.SolutionStacks.Queries;
using OpsPortal.Contracts.Common;
using OpsPortal.Contracts.SolutionStacks;
using OpsPortal.WebApi.Extensions;

namespace OpsPortal.WebApi.Controllers;

[ApiController]
[Route("api/solution-stacks")]
public class SolutionStacksController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolutionStacksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Retrieves a paginated list of solution stacks with optional filtering and sorting
    /// </summary>
    /// <param name="query">Query parameters for pagination, filtering, and sorting</param>
    /// <returns>A paginated response containing solution stack data</returns>
    /// <response code="200">Returns the paginated list of solution stacks</response>
    /// <response code="400">If the query parameters are invalid</response>
    /// <remarks>
    ///     <b>Sample request:</b>
    ///     GET /api/solution-stacks?pageNumber=1&amp;pageSize=20&amp;searchTerm=web&amp;sortBy=name&amp;sortDescending=false
    /// 
    ///     <b>Query Parameters:</b>
    ///     - **PageNumber**: Page number (default: 1, minimum: 1)
    ///     - **PageSize**: Items per page (default: 20, maximum: 100)
    ///     - **SearchTerm**: Optional search term to filter solution stacks by name or description
    ///     - **SortBy**: Field name to sort by (e.g., "name", "category", "updatedAt")
    ///     - **SortDescending**: Sort direction (default: false for ascending)
    ///
    ///     <b>Response:</b>
    ///     The response includes pagination metadata in both the response body and HTTP headers.
    /// 
    ///     <b>Response Headers:</b>
    ///     - <c>X-Pagination</c>: JSON object with <c>TotalCount</c>, <c>PageSize</c>, <c>PageNumber</c>, <c>TotalPages</c>, <c>HasNextPage</c>, <c>HasPreviousPage</c>
    ///     - <c>Link</c>: RFC5988-compliant navigation links for <c>next</c> and <c>prev</c> pages
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<SolutionStackResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<SolutionStackResponse>>> GetAll(
        [FromQuery]GetAllSolutionStacks query)
    {
        var result = await _mediator.Send(query);

        Response.AddPaginationHeaders(result, Request);

        return Ok(result);
    }

    /// <summary>
    ///     Retrieves a solution stack by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the solution stack.</param>
    /// <returns>The solution stack with the specified ID, if found.</returns>
    /// <response code="200">Returns the solution stack with the specified ID.</response>
    /// <response code="404">If a solution stack with the specified ID is not found.</response>
    /// <remarks>
    ///     <b>Sample request:</b>
    ///     GET /api/solution-stacks/{id}
    ///     Replace <c>{id}</c> with the GUID of the solution stack to retrieve.
    ///
    ///     <b>Response:</b>
    ///     The solution stack details will be returned in the response body if found.
    ///
    ///     <b>Response Headers:</b>
    ///     None
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(SolutionStackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Route("{id:guid}")]
    public async Task<ActionResult<SolutionStackResponse>> GetById([FromRoute]Guid id)
    {
        var result = await _mediator.Send(new GetSolutionStackById(id));

        if (result == null) return NotFound();

        return Ok(result);
    }
}
