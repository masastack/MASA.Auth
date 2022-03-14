using Masa.Contrib.BasicAbilities.Auth.Callers;
using Masa.Contrib.BasicAbilities.Auth.Callers.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Callers.Permissions;

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
