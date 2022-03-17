namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}
