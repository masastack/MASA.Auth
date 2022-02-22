namespace MASA.Auth.Caller.Callers;

internal class PlatformCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }

    internal PlatformCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(PlatformCaller);
        BaseAddress = "";
    }
}
