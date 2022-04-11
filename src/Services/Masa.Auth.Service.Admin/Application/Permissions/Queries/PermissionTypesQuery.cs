namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionTypesQuery : Query<List<SelectItemDto<int>>>
{
    public override List<SelectItemDto<int>> Result { get; set; } = new();
}
