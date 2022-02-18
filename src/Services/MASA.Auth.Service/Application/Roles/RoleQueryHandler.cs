namespace MASA.Auth.Service.Application
{
    public class RoleQueryHandler
    {
        readonly IRepository<Role> _roleRepository;
        public RoleQueryHandler(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
    }
}