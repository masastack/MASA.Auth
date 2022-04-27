namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiScopeQuery(int Page, int PageSize, string Search) : Query<PaginationDto<ApiScopeDto>>
{
    public override PaginationDto<ApiScopeDto> Result { get; set; } = new();
}
