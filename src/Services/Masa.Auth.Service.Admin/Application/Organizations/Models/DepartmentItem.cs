namespace Masa.Auth.Service.Application.Organizations.Models;

public class DepartmentItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentItem> Children { get; set; } = new();
}

