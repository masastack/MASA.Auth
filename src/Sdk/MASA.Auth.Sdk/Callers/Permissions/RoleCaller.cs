namespace MASA.Auth.Caller.Callers;

public class RoleCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public RoleCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(PlatformCaller);
        BaseAddress = "";
    }
}
