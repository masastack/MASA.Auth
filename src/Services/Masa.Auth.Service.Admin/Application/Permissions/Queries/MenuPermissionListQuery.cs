namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record MenuPermissionListQuery(int SystemId) : Query<List<AppPermissionDto>>
{
    public override List<AppPermissionDto> Result { get; set; } = new();
}
