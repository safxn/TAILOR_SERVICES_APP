namespace DomainLayer.Common;

public class OperationResult
{
    public bool Succeeded => Errors.Count == 0;
    public List<string> Errors { get; } = [];

    public static OperationResult Ok() => new();
    public static OperationResult Fail(params string[] errors)
    {
        var r = new OperationResult();
        r.Errors.AddRange(errors);
        return r;
    }
}

public class OperationResult<T> : OperationResult
{
    public T? Value { get; private set; }

    public static OperationResult<T> Ok(T value) => new() { Value = value };
    public new static OperationResult<T> Fail(params string[] errors)
    {
        var r = new OperationResult<T>();
        r.Errors.AddRange(errors);
        return r;
    }
}