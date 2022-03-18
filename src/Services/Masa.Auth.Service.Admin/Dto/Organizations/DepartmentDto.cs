namespace Masa.Auth.Service.Admin.Dto.Organizations;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentDto> Children { get; set; } = new();
}

