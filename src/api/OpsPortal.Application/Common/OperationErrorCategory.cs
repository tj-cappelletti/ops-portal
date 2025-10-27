namespace OpsPortal.Application.Common;

public enum OperationErrorCategory
{
    /// <summary>
    ///     Validation errors, such as invalid input data.
    /// </summary>
    Validation,

    /// <summary>
    ///     A required resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    ///     A conflict occurred, such as a duplicate resource.
    /// </summary>
    Conflict,

    /// <summary>
    ///     User's authentication failed or user is not authenticated.
    /// </summary>
    Unauthorized,

    /// <summary>
    ///     User is authenticated but does not have permission to perform the operation.
    /// </summary>
    Forbidden,

    /// <summary>
    ///     A business rule was violated.
    /// </summary>
    BusinessRule,

    /// <summary>
    ///     An unexpected error occurred related to the application's internal logic.
    /// </summary>
    Internal,

    /// <summary>
    ///     An error originating from an external system or service.
    /// </summary>
    External
}
