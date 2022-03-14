using Masa.Auth.ApiGateways.Caller.Callers.Subjects;

namespace Masa.Auth.ApiGateways.Caller.Callers.Permissions;

public class RoleCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    public RoleCaller(IServiceProvider serviceProvider, Options options) : base(serviceProvider)
    {
        Name = nameof(ThirdPartyIdpCaller);
        BaseAddress = options.AuthServiceBaseAdress;
    }
}
