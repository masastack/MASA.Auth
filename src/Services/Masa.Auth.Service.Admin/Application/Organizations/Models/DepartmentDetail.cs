namespace Masa.Auth.Service.Admin.Application.Organizations.Models;

public class DepartmentDetail
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; }
}

