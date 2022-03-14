namespace Masa.Auth.Service.Application.Permissions.Queries;

public record ApiPermissionsQuery(int SystemId) : Query<List<AppPermissionItem>>
{
    public override List<AppPermissionItem> Result { get; set; } = null!;
}
