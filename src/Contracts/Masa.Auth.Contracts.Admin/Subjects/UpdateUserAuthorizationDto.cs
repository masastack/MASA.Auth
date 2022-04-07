namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserAuthorizationDto
{
    public Guid Id { get; set; }

    public List<Guid> Roles { get; set; }

    public List<UserPermissionDto> Permissions { get; set; }

    public UpdateUserAuthorizationDto(Guid id, List<Guid> roles, List<UserPermissionDto> permissions)
    {
        Id = id;
        Roles = roles;
        Permissions = permissions;
    }
}

