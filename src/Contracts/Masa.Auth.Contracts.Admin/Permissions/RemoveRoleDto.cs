namespace Masa.Auth.Contracts.Admin.Permissions;

public class RemoveRoleDto
{
    public Guid Id { get; set; }

    public RemoveRoleDto(Guid id)
    {
        Id = id;
    }
}

