namespace Masa.Auth.Contracts.Admin.Sso;

public class IdentityResourceSelectDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public IdentityResourceSelectDto(int id, string name, string displayName, string description)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Description = description;
    }
}

