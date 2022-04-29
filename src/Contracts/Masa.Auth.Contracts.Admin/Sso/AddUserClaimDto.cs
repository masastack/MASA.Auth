namespace Masa.Auth.Contracts.Admin.Sso;

public class AddUserClaimDto
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public UserClaimType UserClaimType { get; set; }

    public AddUserClaimDto()
    {
    }

    public AddUserClaimDto(string name, string description, UserClaimType userClaimType)
    {
        Name = name;
        Description = description;
        UserClaimType = userClaimType;
    }
}

