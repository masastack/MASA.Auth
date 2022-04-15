namespace Masa.Auth.Service.Admin.Domain.Permissions.Events;

public record UpdateRoleLimitDomainEvent(IEnumerable<Guid> Roles) : Event;

