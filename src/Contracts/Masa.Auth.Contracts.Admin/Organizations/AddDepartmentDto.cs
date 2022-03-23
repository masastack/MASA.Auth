namespace Masa.Auth.Contracts.Admin.Organizations;

public class AddDepartmentDto
{
    public string Name { get; set; } = "";

    public Guid ParentId { get; set; }

    public List<Guid> StaffIds { get; set; } = new();

    public string Description { get; set; } = "";

    public bool MigrateStaff { get; set; }

    public bool Enabled { get; set; }
}

