namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandler;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly IRoleRepository _roleRepository;

    public SetTeamPersonnelInfoDomainEventHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [EventHandler(1)]
    public async Task SetRoleAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        var teamRole = setTeamPersonnelInfoDomainEvent.Team.TeamRoles.FirstOrDefault(tr => tr.TeamMemberType == setTeamPersonnelInfoDomainEvent.Type);
        Role role;
        if (teamRole is null)
        {
            role = new Role(setTeamPersonnelInfoDomainEvent.RoleName, setTeamPersonnelInfoDomainEvent.RoleName);
            await _roleRepository.AddAsync(role);
            await _roleRepository.UnitOfWork.SaveChangesAsync();
            setTeamPersonnelInfoDomainEvent.Team.SetRole(setTeamPersonnelInfoDomainEvent.Type, role.Id);
        }
        else
        {
            role = await _roleRepository.GetByIdAsync(teamRole.RoleId);
        }
        role.BindChildrenRoles(setTeamPersonnelInfoDomainEvent.RoleIds);
        await _roleRepository.UpdateAsync(role);
        await _roleRepository.UnitOfWork.SaveChangesAsync();
    }

    [EventHandler(3)]
    public async Task SetPermissionAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        setTeamPersonnelInfoDomainEvent.Team.SetPermission(setTeamPersonnelInfoDomainEvent.Type
                , setTeamPersonnelInfoDomainEvent.PermissionsIds);
        await Task.CompletedTask;
    }
}
