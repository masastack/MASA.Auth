using Masa.Auth.Service.Admin.Application.Permissions.Models;

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record FuncPermissionListQuery(int SystemId) : Query<List<AppPermissionItem>>
{
    public override List<AppPermissionItem> Result { get; set; } = new();
}
