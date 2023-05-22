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
                                    .Where(r => r.Limit != 0 && roleEvent.Roles.Contains(r.Id))
                                    .Include(r => r.Users)
                                    .Include(r => r.Teams)
                                    .ThenInclude(teamUser => teamUser.Team)
                                    .ThenInclude(t => t.TeamStaffs)
                                    .AsSplitQuery()
                                    .ToListAsync();
            foreach (var role in roles)
            {
                var availableQuantity = GetAvailableQuantity(role);
                if (availableQuantity >= 0)
                    role.UpdateAvailableQuantity(availableQuantity);
                else
                    throw new UserFriendlyException(UserFriendlyExceptionCodes.ROLE_BIND_LIMIT_ERROR, role.Name, role.Limit, role.Limit - availableQuantity);
            }

            await _roleRepository.UpdateRangeAsync(roles);

            int GetAvailableQuantity(Role role)
            {
                var availableQuantity = role.Limit - role.Users.Where(user => user.IsDeleted == false).Count();
                if (role.Teams.Count > 0)
                {
                    foreach (var teamRole in role.Teams)
                    {
                        availableQuantity -= teamRole?.Team?.TeamStaffs?.Where(ts => ts.IsDeleted == false && role.Users.Any(user => user.UserId == ts.UserId) == false && ts.TeamMemberType == teamRole.TeamMemberType)?.Count() ?? 0;
                    }
                }

                return availableQuantity;
            }
        }
    }
}
