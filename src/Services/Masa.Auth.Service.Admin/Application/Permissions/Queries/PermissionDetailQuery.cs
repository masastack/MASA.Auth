using Masa.Auth.Service.Admin.Application.Permissions.Models;

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionDetailQuery(Guid PermissionId) : Query<PermissionDetail>
{
    public override PermissionDetail Result { get; set; } = new();
}
