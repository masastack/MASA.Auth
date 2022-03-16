namespace Masa.Auth.Service.Services;

public class RoleService : ServiceBase
{
    public RoleService(IServiceCollection services) : base(services, Routing.ROLE_BASE_URI)
    {

    }
}

