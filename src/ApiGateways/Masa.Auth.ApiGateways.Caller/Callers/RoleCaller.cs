namespace Masa.Auth.Caller.Callers;

public class RoleCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; } = "http://localhost:6102";

    public override string Name { get; set; }

    public RoleCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(RoleCaller);
    }
}
