namespace Masa.Auth.Sdk.Response;

public class ApiResultResponse
{
    public bool Success { get; }

    public string Message { get; }

    protected ApiResultResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static ApiResultResponse ResponseSuccess(string message) => new(true, message);

    public static ApiResultResponse ResponseLose(string message) => new(false, message);
}

public class ApiResultResponse<TEntity> : ApiResultResponse
{
    public TEntity? Data { get; }

    private ApiResultResponse(bool success, string message, TEntity? data) : base(success, message)
    {
        Data = data;
    }

    public static ApiResultResponse<TEntity> ResponseSuccess(TEntity? data, string message) => new(true, message, data);

    public static ApiResultResponse<TEntity> ResponseLose(string message, TEntity? data) => new(false, message, data);
}
