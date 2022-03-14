namespace Masa.Auth.Service.Application.Permissions.Queries;

public record ApiPermissionListQuery(int SystemId) : Query<List<AppPermissionItem>>
{
    public override List<AppPermissionItem> Result { get; set; } = null!;
}
