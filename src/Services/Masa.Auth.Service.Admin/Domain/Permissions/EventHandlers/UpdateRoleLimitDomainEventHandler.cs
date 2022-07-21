// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Service.Admin.Domain.Permissions.Events;

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandlers
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
                                    .ThenInclude(teamUser => teamUser.Team)
                                    .ThenInclude(t => t.TeamStaffs)
                                    .Where(r => r.Limit != 0 && roleEvent.Roles.Contains(r.Id))
                                    .ToListAsync();
            foreach (var role in roles)
            {
                var availableQuantity = GetAvailableQuantity(role);
                if (availableQuantity >= 0)
                    role.UpdateAvailableQuantity(availableQuantity);
                else
                    throw new UserFriendlyException($"角色：{role.Name} 超出绑定限制，最多只能绑定{role.Limit}人!");
            }

            await _roleRepository.UpdateRangeAsync(roles);

            int GetAvailableQuantity(Role role)
            {
                var availableQuantity = role.Limit - role.Users.Count;
                if (role.Teams.Count > 0)
                {
                    foreach (var teamRole in role.Teams)
                    {
                        availableQuantity -= teamRole.Team.TeamStaffs.Where(ts => ts.TeamMemberType == teamRole.TeamMemberType).Count();
                    }
                }

                return availableQuantity;
            }
        }
    }
}
