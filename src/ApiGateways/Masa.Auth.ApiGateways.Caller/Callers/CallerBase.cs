using Masa.Auth.ApiGateways.Caller.Response;

namespace Masa.Auth.ApiGateways.Caller.Callers;

public abstract class CallerBase : HttpClientCallerBase
{
    protected CallerBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task<ApiResultResponse<TResponse>> ResultAsync<TResponse>(Func<Task<TResponse>> func)
    {
        try
        {
            var response = await func.Invoke();
            return ApiResultResponse<TResponse>.ResponseSuccess(response, "success");
        }
        catch (Exception ex)
        {
            if (ex is UserFriendlyException) return ApiResultResponse<TResponse>.ResponseLose(ex.Message);
            return ApiResultResponse<TResponse>.ResponseLose("The service is abnormal, please contact the administrator!");
        }
    }

    protected async Task<ApiResultResponse> ResultAsync(Func<Task<HttpResponseMessage>> func)
    {
        try
        {
            var response = await func.Invoke();
            return response.StatusCode switch
            {
                HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.NoContent => ApiResultResponse.ResponseSuccess("success"),
                (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException => ApiResultResponse.ResponseLose(await response.Content.ReadAsStringAsync()),
                _ => ApiResultResponse.ResponseLose("The service is abnormal, please contact the administrator!"),
            };
        }
        catch (Exception ex)
        {
            return ApiResultResponse.ResponseLose("The service is abnormal, please contact the administrator!");
        }
    }
}
