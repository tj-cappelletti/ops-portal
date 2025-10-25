using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Common;
using OpsPortal.WebApi.Middleware;

namespace OpsPortal.WebApi.Controllers;

public abstract class ApiControllerBase<TController> : ControllerBase
{
    protected ILogger<TController> Logger;

    public string? CorrelationId => (string?)HttpContext.Items[CorrelationIdMiddleware.CorrelationIdContextKey];

    protected ApiControllerBase(ILogger<TController> logger)
    {
        Logger = logger;
    }

    private ProblemDetails CreateProblemDetails(OperationError error)
    {
        Logger.LogInformation("Operation failed with error code '{errorCode}'", error.Code);
        Logger.LogDebug("Operation failed with error details: {errorDetails}", error);
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Extensions = error.Metadata?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>()
        };
    }

    private IActionResult CreateValidationResponse(OperationError error)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation Failed",
            Detail = error.Message
        };

        if (error.Metadata?.TryGetValue(OperationError.MetadataErrorsKey, out var errors) == true &&
            errors is Dictionary<string, string[]> validationErrors)
            foreach (var kvp in validationErrors)
                problemDetails.Errors[kvp.Key] = kvp.Value;

        return BadRequest(problemDetails);
    }

    protected IActionResult HandleOperationError(OperationError? error)
    {
        if (error == null)
        {
            Logger.LogError("OperationError is null in HandleOperationError");

            return StatusCode(500, new ProblemDetails
            {
                Title = "InternalServerError",
                Detail = "An unknown error occurred"
            });
        }

        return error.Category switch
        {
            OperationErrorCategory.Validation => CreateValidationResponse(error),
            OperationErrorCategory.NotFound => NotFound(CreateProblemDetails(error)),
            OperationErrorCategory.Conflict => Conflict(CreateProblemDetails(error)),
            OperationErrorCategory.Unauthorized => Unauthorized(CreateProblemDetails(error)),
            OperationErrorCategory.BusinessRule => UnprocessableEntity(CreateProblemDetails(error)),
            _ => StatusCode(500, CreateProblemDetails(error))
        };
    }

    protected IActionResult HandleOperationResult<T>(OperationResult<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleOperationError(result.Error!);
    }
}
