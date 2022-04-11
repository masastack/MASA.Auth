namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record ApiPermissionDetailQuery(Guid PermissionId) : Query<ApiPermissionDetailDto>
{
    public override ApiPermissionDetailDto Result { get; set; } = new();
}