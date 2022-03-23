namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RolePaginationQuery(int Page, int PageSize, string Search, bool Enabled) : Query<PaginationDto<RoleDto>>
{
    public override PaginationDto<RoleDto> Result { get; set; } = new();
}
