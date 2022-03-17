namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class PermissionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public List<PermissionDto> Children { get; set; } = new();
}
