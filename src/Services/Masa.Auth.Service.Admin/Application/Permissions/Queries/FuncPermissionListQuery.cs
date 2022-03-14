namespace Masa.Auth.Service.Application.Permissions.Queries;

public record FuncPermissionListQuery(int SystemId) : Query<List<AppPermissionItem>>
{
    public override List<AppPermissionItem> Result { get; set; } = new();
}
