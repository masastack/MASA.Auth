namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public RoleSelectItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
