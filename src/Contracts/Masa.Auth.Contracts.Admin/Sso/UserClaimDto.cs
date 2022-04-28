namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public UserClaimType UserClaimType { get; set; }

    public UserClaimDto()
    {
    }

    public UserClaimDto(int id, string name, string description, UserClaimType userClaimType)
    {
        Id = id;
        Name = name;
        Description = description;
        UserClaimType = userClaimType;
    }
}

