namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record RemoveUserDomainEvent(Guid userId) : Event;
