namespace MASA.Auth.Service.Application.Organizations.Commands;

public record CreateDepartmentCommand(string Name, string Description, Guid ParentId, List<Guid> StaffIds, bool Enabled = true) : Command
{
}

