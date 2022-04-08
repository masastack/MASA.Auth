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

    protected async Task<TResponse> GetAsync<TResponse>(string methodName, Dictionary<string, string>? paramters = null)
    {
        return await CallerProvider.GetAsync<TResponse>(BuildAdress(methodName), paramters ?? new()) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task PutAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PutAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        var response = await CallerProvider.PostAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task DeleteAsync<TRequest>(string methodName, TRequest? data = default)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), data);
        await CheckResponse(response);
    }

    protected async Task DeleteAsync(string methodName)
    {
        var response = await CallerProvider.DeleteAsync(BuildAdress(methodName), null);
        await CheckResponse(response);
    }

    protected async Task SendAsync<TRequest>(string methodName, TRequest? data = default)
    {
        if (methodName.StartsWith("Add")) await PutAsync(methodName, data);
        else if (methodName.StartsWith("Update")) await PostAsync(methodName, data);
        else if (methodName.StartsWith("Remove")) await DeleteAsync(methodName, data);
    }

    protected async Task<TResponse> SendAsync<TResponse>(string methodName, Dictionary<string, string>? data = null)
    {
        return await GetAsync<TResponse>(methodName, data);
    }

    string BuildAdress(string methodName)
    {
        return Path.Combine(BaseUrl, methodName.Replace("Async", ""));
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

