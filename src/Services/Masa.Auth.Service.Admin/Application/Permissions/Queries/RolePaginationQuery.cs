namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RolePaginationQuery(int PageIndex, int PageSize, string Search, bool Enabled) : Query<PaginationList<RoleDto>>
{
    public override PaginationList<RoleDto> Result { get; set; } = new();
}
