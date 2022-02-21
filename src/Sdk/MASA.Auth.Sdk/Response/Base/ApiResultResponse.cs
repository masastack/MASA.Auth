namespace MASA.Auth.Sdk.Response;

public class ApiResultResponse<TEntity> : ApiResultResponseBase
{
    public TEntity? Data { get; }

    private ApiResultResponse(bool success, string message, TEntity? data) : base(success, message)
    {
        Data = data;
    }

    public static ApiResultResponse<TEntity> ResponseSuccess(TEntity? data, string message) => new(true, message, data);

    public static ApiResultResponse<TEntity> ResponseLose(string message, TEntity? data) => new(false, message, data);
}
