namespace Masa.Auth.Contracts.Admin.Organizations;

public class CopyDepartmentDto : AddOrUpdateDepartmentDto
{
    public List<Guid> StaffIds { get; set; } = new();

    public bool MigrateStaff { get; set; }

    public Guid SourceId { get; set; }
}

