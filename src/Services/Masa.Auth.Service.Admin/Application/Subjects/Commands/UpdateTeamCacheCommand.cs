namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record UpdateTeamCacheCommand(List<Guid> TeamIds) : Command;