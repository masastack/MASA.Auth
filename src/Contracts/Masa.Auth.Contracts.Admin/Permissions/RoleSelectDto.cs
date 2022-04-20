namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Limit { get; set; }

    public int AvailableQuantity { get; set; }

    public RoleSelectDto(Guid id, string name, int limit, int availableQuantity)
    {
        Id = id;
        Name = name;
        Limit = limit;
        AvailableQuantity = availableQuantity;
    }
}
