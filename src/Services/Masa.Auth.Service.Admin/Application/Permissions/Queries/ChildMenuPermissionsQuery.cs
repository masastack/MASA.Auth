namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ChildMenuPermissionsQuery(Guid PermissionId) : Query<List<PermissionDto>>
{
    public override List<PermissionDto> Result { get; set; } = new();
}

