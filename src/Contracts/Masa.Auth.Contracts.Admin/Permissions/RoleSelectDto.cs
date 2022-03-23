namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public RoleSelectDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
