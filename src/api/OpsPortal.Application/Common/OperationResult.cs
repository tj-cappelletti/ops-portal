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

    public static OperationResult<TValue> Failure(OperationError error)
    {
        return new OperationResult<TValue>(default, error, false);
    }

    //public OperationResult<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> mapper)
    //{
    //    return IsSuccess
    //        ? OperationResult<TNewValue>.Success(mapper(Value!))
    //        : OperationResult<TNewValue>.Failure(Error!);
    //}

    //public async Task<OperationResult<TNewValue>> MapAsync<TNewValue>(
    //    Func<TValue, Task<TNewValue>> mapper)
    //{
    //    return IsSuccess
    //        ? OperationResult<TNewValue>.Success(await mapper(Value!))
    //        : OperationResult<TNewValue>.Failure(Error!);
    //}

    public static implicit operator OperationResult<TValue>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator OperationResult<TValue>(OperationError error)
    {
        return Failure(error);
    }

    public static OperationResult<TValue> Success(TValue value)
    {
        return new OperationResult<TValue>(value, null, true);
    }
}
