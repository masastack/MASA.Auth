namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserPermissionDto
{
    public Guid PermissionId { get; set; }

    public bool Effect { get; set; }

    public UserPermissionDto(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }
}

