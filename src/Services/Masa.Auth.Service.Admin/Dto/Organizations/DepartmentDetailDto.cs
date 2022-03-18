namespace Masa.Auth.Service.Admin.Dto.Organizations;

public class DepartmentDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; }
}

