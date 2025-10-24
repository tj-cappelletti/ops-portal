namespace OpsPortal.Application.Common;

public record BusinessValidationResult(bool IsValid, BusinessValidationError[] Errors)
{
    public static BusinessValidationResult Failed(params BusinessValidationError[] errors)
    {
        return new BusinessValidationResult(false, errors);
    }

    public static BusinessValidationResult Success()
    {
        return new BusinessValidationResult(true, Array.Empty<BusinessValidationError>());
    }
}
