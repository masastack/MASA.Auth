namespace Masa.Auth.Contracts.Admin.Organizations;

public class UpsertDepartmentDto : BaseUpsertDto<Guid>
{

    public string Name { get; set; } = "";

    public Guid ParentId { get; set; }

    public string Description { get; set; } = "";

    public bool Enabled { get; set; } = true;

    public List<Guid> StaffIds { get; set; } = new();
}

