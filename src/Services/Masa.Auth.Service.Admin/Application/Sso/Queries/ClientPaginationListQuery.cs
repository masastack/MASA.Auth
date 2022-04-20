namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientPaginationListQuery(int Page, int PageSize) : Query<PaginationDto<ClientDto>>
{
    public override PaginationDto<ClientDto> Result { get; set; } = new();
}
