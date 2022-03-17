namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ApiPermissionListQuery(int SystemId) : Query<List<AppPermissionDto>>
{
    public override List<AppPermissionDto> Result { get; set; } = null!;
}
