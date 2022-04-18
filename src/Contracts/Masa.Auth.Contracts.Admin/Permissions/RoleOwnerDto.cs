namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleOwnerDto
{
    public List<UserSelectDto> Users { get; set; }

    public List<TeamSelectDto> Teams { get; set; }

    public RoleOwnerDto()
    {
        Users = new();
        Teams = new();
    }

    public RoleOwnerDto(List<UserSelectDto> users, List<TeamSelectDto> teams)
    {
        Users = users;
        Teams = teams;
    }
}

