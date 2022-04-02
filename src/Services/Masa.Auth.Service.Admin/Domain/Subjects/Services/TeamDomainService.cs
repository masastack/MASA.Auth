namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class TeamDomainService : DomainService
{
    public TeamDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task SetTeamAdminAsync(Team team, List<Guid> staffIds, List<Guid> roleIds, Dictionary<Guid, bool> permissionsIds)
    {
        var _event = new SetTeamPersonnelInfoDomainEvent(team, TeamMemberTypes.Admin, $"{team.Name}管理员", staffIds, roleIds, permissionsIds);
        await EventBus.PublishAsync(_event);
    }

    public async Task SetTeamMemberAsync(Team team, List<Guid> staffIds, List<Guid> roleIds, Dictionary<Guid, bool> permissionsIds)
    {
        var _event = new SetTeamPersonnelInfoDomainEvent(team, TeamMemberTypes.Member, $"{team.Name}成员", staffIds, roleIds, permissionsIds);
        await EventBus.PublishAsync(_event);
    }
}
