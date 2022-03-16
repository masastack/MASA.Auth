namespace Masa.Auth.ApiGateways.Caller.Services;

public abstract class ServiceBase
{
    ICallerProvider CallerProvider { get; init;}

    protected ServiceBase(ICallerProvider callerProvider)
    {
        CallerProvider = callerProvider;
    }
}

