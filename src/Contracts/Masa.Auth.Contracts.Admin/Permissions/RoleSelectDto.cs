namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int QuantityAvailable { get; set; }

    public RoleSelectDto(Guid id, string name, int quantityAvailable)
    {
        Id = id;
        Name = name;
        QuantityAvailable = quantityAvailable;
    }
}
