namespace Masa.Auth.Sdk.Callers.Permissions;

public class RoleCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public RoleCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(PlatformCaller);
        BaseAddress = "";
    }
}
