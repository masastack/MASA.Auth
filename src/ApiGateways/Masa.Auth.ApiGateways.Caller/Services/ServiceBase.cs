namespace Masa.Auth.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    ICallerProvider CallerProvider { get; init; }

    protected ServiceBase(ICallerProvider callerProvider)
    {
        CallerProvider = callerProvider;
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName)
    {
        return await CallerProvider.GetAsync<TResponse>(methodName) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task<TResponse> GetAsync<TResponse>(string methodName, Dictionary<string, string> paramters)
    {
        return await CallerProvider.GetAsync<TResponse>(methodName, paramters) ?? throw new Exception("The service is abnormal, please contact the administrator!");
    }

    protected async Task PostAsync<TRequest>(string methodName, TRequest data)
    {
        await CallerProvider.PostAsync(methodName, data);
    }

    protected async Task DeleteAsync(string methodName)
    {
        await CallerProvider.DeleteAsync(methodName, null);
    }
}

