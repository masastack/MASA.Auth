using Masa.Auth.Service.Admin.Domain.Permissions.Events;

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandler
{
    public class UpdateRoleLimitDomainEventHandler
    {
        readonly IRoleRepository _roleRepository;
        readonly AuthDbContext _authDbContext;

        public UpdateRoleLimitDomainEventHandler(IRoleRepository roleRepository, AuthDbContext authDbContext)
        {
            _roleRepository = roleRepository;
            _authDbContext = authDbContext;
        }

        [EventHandler(1)]
        public async Task UpdateRoleLimitAsync(UpdateRoleLimitDomainEvent roleEvent)
        {
            var roles = await _authDbContext.Set<Role>()
                                    .Include(r => r.Users)
                                    .Include(r => r.Teams)
                                    .ThenInclude(tr => tr.Team)
                                    .ThenInclude(t => t.TeamStaffs)
                                    .Where(r => roleEvent.Roles.Contains(r.Id))
                                    .ToListAsync();
            foreach (var role in roles)
            {
                var quantityAvailable = GetQuantityAvailable(role);
                if (role.Limit == 0 || quantityAvailable >= 0)
                    role.UpdateQuantityAvailable(quantityAvailable);
                else
                    throw new UserFriendlyException($"Role：{role.Name} exceed the limit!");
            }

            await _roleRepository.UpdateRangeAsync(roles);

            int GetQuantityAvailable(Role role)
            {
                var quantityAvailable = role.Limit - role.Users.Count;
                if (role.Teams.Count > 0)
                {
                    foreach(var teamRole in role.Teams)
                    {
                        if(teamRole.TeamMemberType == TeamMemberTypes.Admin)
                        {
                            quantityAvailable -= teamRole.Team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Admin).Count();
                        }
                        else if(teamRole.TeamMemberType == TeamMemberTypes.Member)
                        {
                            quantityAvailable -= teamRole.Team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Member).Count();
                        }
                    }
                }

                return quantityAvailable;
            }
        }
    }
}
