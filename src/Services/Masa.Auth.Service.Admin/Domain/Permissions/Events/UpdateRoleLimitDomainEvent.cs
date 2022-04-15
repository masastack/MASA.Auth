namespace Masa.Auth.Service.Admin.Domain.Permissions.Events;

public record UpdateRoleLimitDomainEvent(List<Guid> Roles, int teamUserCount) : Event;

