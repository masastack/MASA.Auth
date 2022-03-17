namespace Masa.Auth.Service.Application.Permissions.Models;

public class RoleSelectItem
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public RoleSelectItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
