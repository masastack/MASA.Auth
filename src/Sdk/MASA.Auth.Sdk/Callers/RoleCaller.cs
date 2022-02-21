namespace MASA.Auth.Caller.Callers;

public class RoleCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }

    public RoleCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(RoleCaller);
        BaseAddress = "";
    }
}
