namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record UpdateTeamPersonnelCommand(UpdateTeamPersonnelDto UpdateTeamPersonnelDto, TeamMemberTypes MemberType) : Command
{
}
