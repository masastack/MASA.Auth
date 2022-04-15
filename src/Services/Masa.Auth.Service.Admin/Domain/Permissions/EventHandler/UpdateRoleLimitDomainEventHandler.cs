using Masa.Auth.Service.Admin.Domain.Permissions.Events;

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandler
{
    public class UpdateRoleLimitDomainEventHandler
    {
        readonly IRoleRepository _roleRepository;
        readonly ITeamRepository _tamRepository;
        readonly AuthDbContext _authDbContext;

        public UpdateRoleLimitDomainEventHandler(IRoleRepository roleRepository, ITeamRepository tamRepository, AuthDbContext authDbContext)
        {
            _roleRepository = roleRepository;
            _tamRepository = tamRepository;
            _authDbContext = authDbContext;
        }

        [EventHandler(1)]
        public async Task UpdateRoleLimitAsync(UpdateRoleLimitDomainEvent roleEvent)
        {
            var roles = await _authDbContext.Set<Role>()
                                    .Include(r => r.Users)
                                    .Where(r => roleEvent.Roles.Contains(r.Id))
                                    .ToListAsync();
            foreach (var role in roles)
            {
                if (role.Limit != 0)
                {
                    var quantityAvailable = (int)role.Limit - role.Users.Count - roleEvent.teamUserCount;
                    if (quantityAvailable >= 0)
                        role.UpdateQuantityAvailable((int)quantityAvailable);
                    else
                        throw new UserFriendlyException($"Role：{role.Name} exceed the limit!");
                }
            }

            await _roleRepository.UpdateRangeAsync(roles);
        }
    }
}
