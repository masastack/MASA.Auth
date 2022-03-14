namespace Masa.Auth.Service.Application.Organizations.Commands;

public record AddDepartmentCommand(string Name, string Description, Guid ParentId, List<Guid> StaffIds, bool Enabled = true) : Command
{
}

