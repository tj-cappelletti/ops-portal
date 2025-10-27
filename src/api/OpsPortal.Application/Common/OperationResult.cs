namespace OpsPortal.Application.Common;

public class OperationResult<TValue>
{
    public OperationError? Error { get; }

    public bool IsFailure => !IsSuccess;

    public bool IsSuccess { get; }

    public TValue? Value { get; }

    protected OperationResult(TValue? value, OperationError? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    private static OperationResult<TValue> Failure(OperationError error)
    {
        return new OperationResult<TValue>(default, error, false);
    }

    public static implicit operator OperationResult<TValue>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator OperationResult<TValue>(OperationError error)
    {
        return Failure(error);
    }

    private static OperationResult<TValue> Success(TValue value)
    {
        return new OperationResult<TValue>(value, null, true);
    }
}
