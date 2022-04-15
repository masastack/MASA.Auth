namespace Masa.Auth.Contracts.Admin.Organizations;

public class DepartmentDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<StaffDto> StaffList { get; set; } = new();
}

