using MASA.Auth.Sdk.Callers;
using MASA.Auth.Sdk.Callers.Subjects;

namespace MASA.Auth.Sdk.Callers.Permissions;

public class RoleCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public RoleCaller(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Name = nameof(PlatformCaller);
        BaseAddress = "";
    }
}
