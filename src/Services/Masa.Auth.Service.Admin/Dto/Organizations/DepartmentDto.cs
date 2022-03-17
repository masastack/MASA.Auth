namespace Masa.Auth.Service.Admin.Application.Organizations.Models;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentDto> Children { get; set; } = new();
}

