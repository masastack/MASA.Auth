using Masa.Utils.Exceptions;
using System.Net;

namespace Masa.Auth.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    protected ICallerProvider CallerProvider { get; init; }

    protected abstract string BaseUrl { get; set; }

    protected ServiceBase(ICallerProvider callerProvider)
    {
        CallerProvider = callerProvider;
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName, Dictionary<string, string> paramters)
    {
        return await CallerProvider.GetAsync<TResponse>(Path.Combine(BaseUrl, methodName), paramters) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task PutAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PutAsync(Path.Combine(BaseUrl, methodName), data);
        await CheckResponse(response);
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PostAsync(Path.Combine(BaseUrl, methodName), data);
        await CheckResponse(response);
    }

    protected async Task DeleteAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.DeleteAsync(Path.Combine(BaseUrl, methodName), data);
        await CheckResponse(response);
    }

    protected async ValueTask CheckResponse(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.NoContent: break;
            case (HttpStatusCode)MasaHttpStatusCode.UserFriendlyException: throw new Exception(await response.Content.ReadAsStringAsync());
            default: throw new Exception("The service is abnormal, please contact the administrator!");
        }
    }
}

