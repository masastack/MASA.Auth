namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiResourceQuery(int Page, int PageSize, string Search) : Query<PaginationDto<ApiResourceDto>>
{
    public override PaginationDto<ApiResourceDto> Result { get; set; } = new();
}
