using MASA.Auth.RolePermission.Domain.RoleRepository;

namespace MASA.Auth.RolePermission.Application.Orders
{
    public class RoleQueryHandler
    {
        readonly IRoleRepository _roleRepository;
        public RoleQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
    }
}