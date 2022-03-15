namespace Masa.Auth.ApiGateways.Caller.Request.Organizations;

public class AddDepartmentRequest
{
    public string Name { get; set; } = "";

    public Guid ParentId { get; set; }

    public List<Guid> StaffIds { get; set; } = new();

    public string Description { get; set; } = "";

    public bool MigrateStaff { get; set; }

    public bool Enabled { get; set; }
}

