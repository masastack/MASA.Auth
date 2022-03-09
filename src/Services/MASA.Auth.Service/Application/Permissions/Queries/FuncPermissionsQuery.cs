namespace Masa.Auth.Service.Application.Permissions.Queries;

public record FuncPermissionsQuery(int SystemId) : Query<List<AppPermissionItem>>
{
    public override List<AppPermissionItem> Result { get; set; } = new();
}
