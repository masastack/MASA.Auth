namespace Masa.Auth.Contracts.Admin.Organizations;

public class CopyDepartmentDto : AddOrUpdateDepartmentDto
{
    public List<StaffDto> Staffs { get; set; } = new();

    public new List<Guid> StaffIds => Staffs.Select(s => s.Id).ToList();

    public bool MigrateStaff { get; set; }

    public Guid SourceId { get; set; }
}

