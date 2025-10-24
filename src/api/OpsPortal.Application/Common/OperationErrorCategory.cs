namespace OpsPortal.Application.Common;

public enum OperationErrorCategory
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    BusinessRule,
    Infrastructure,
    External
}
