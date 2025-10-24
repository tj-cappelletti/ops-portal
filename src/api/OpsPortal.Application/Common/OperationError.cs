namespace OpsPortal.Application.Common;

public class OperationError
{
    public const string MetadataErrorsKey = "errors";

    public OperationErrorCategory Category { get; }

    public string Code { get; }

    public string Message { get; }

    public Dictionary<string, object>? Metadata { get; init; }

    protected OperationError(string code, string message, OperationErrorCategory category)
    {
        Code = code;
        Message = message;
        Category = category;
    }

    public static OperationError CreateBusinessRuleViolationOperationError(string message, object? details = null)
    {
        return new OperationError("BusinessRuleViolation", message, OperationErrorCategory.BusinessRule)
        {
            Metadata = details != null
                ? new Dictionary<string, object> { ["details"] = details }
                : null
        };
    }

    public static OperationError CreateConflictOperationError(string message)
    {
        return new OperationError("Conflict", message, OperationErrorCategory.Conflict);
    }

    public static OperationError CreateNotFoundOperationError(string resource, object id)
    {
        return new OperationError("NotFound", $"{resource} with ID {id} was not found", OperationErrorCategory.NotFound);
    }

    public static OperationError CreateUnauthorizedOperationError(string message = "Unauthorized access")
    {
        return new OperationError("Unauthorized", message, OperationErrorCategory.Unauthorized);
    }

    // Factory methods for common errors
    public static OperationError CreateValidationFailedOperationError(Dictionary<string, string[]> errors)
    {
        return new OperationError("ValidationFailed", "One or more validation errors occurred", OperationErrorCategory.Validation)
        {
            Metadata = new Dictionary<string, object> { [MetadataErrorsKey] = errors }
        };
    }
}
