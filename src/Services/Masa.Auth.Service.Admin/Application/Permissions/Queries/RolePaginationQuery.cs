namespace Masa.Auth.Service.Application.Permissions.Queries;

public record RolePaginationQuery(int PageIndex, int PageSize, string Search, bool Enabled) : Query<PaginationItems<RoleItem>>
{
    public override PaginationItems<RoleItem> Result { get; set; } = new();
}
