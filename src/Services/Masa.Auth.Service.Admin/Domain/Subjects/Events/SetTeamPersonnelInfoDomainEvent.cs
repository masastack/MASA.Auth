namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record SetTeamPersonnelInfoDomainEvent(Team Team, TeamMemberTypes Type, string RoleName,
        List<Guid> StaffIds, List<Guid> RoleIds, Dictionary<Guid, bool> PermissionsIds)
        : Event;
