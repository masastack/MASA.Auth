namespace Masa.Auth.Contracts.Admin.Organizations;

public class AddOrUpdateDepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public Guid ParentId { get; set; }

    public string Description { get; set; } = "";

    public bool Enabled { get; set; } = true;
}

