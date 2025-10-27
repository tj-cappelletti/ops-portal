using Microsoft.AspNetCore.Mvc;
using OpsPortal.Application.Common;
using OpsPortal.WebApi.Middleware;

namespace OpsPortal.WebApi.Controllers;

public abstract class ApiControllerBase<TController> : ControllerBase
{
    protected ILogger<TController> Logger;

    protected ApiControllerBase(ILogger<TController> logger)
    {
        Logger = logger;
    }

    private ProblemDetails CreateProblemDetails(OperationError error)
    {
        Logger.LogInformation("Creating ProblemDetails");
        return new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Message,
            Extensions = error.Metadata?.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? new Dictionary<string, object?>()
        };
    }

    private IActionResult CreateValidationResponse(OperationError error)
    {
        Logger.LogInformation("Creating ValidationProblemDetails");

        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation Failed",
            Detail = error.Message
        };

        if (error.Metadata?.TryGetValue(OperationError.MetadataErrorsKey, out var errors) == true &&
            errors is Dictionary<string, string[]> validationErrors)
        {
            Logger.LogDebug("Adding validation errors to ProblemDetails: {validationErrors}", validationErrors);
            foreach (var keyValuePair in validationErrors)
                problemDetails.Errors[keyValuePair.Key] = keyValuePair.Value;
        }
        else
        {
            Logger.LogWarning("No validation errors found in OperationError metadata");
        }

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

        Logger.LogInformation("Operation failed with error code '{errorCode}'", error.Code);
        Logger.LogDebug("Operation failed with error details: {errorDetails}", error.Message);
        foreach (var kvp in error.Metadata ?? new Dictionary<string, object>())
            Logger.LogDebug("Error metadata - {key}: {value}", kvp.Key, kvp.Value);

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
        if (result.IsSuccess)
        {
            Logger.LogInformation("Operation succeeded");
            return Ok(result.Value!);
        }
        
        Logger.LogInformation("Operation failed");
        return HandleOperationError(result.Error);
    }
}
