namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record UserClaimQuery(int Page, int PageSize, string Search) : Query<PaginationDto<UserClaimDto>>
{
    public override PaginationDto<UserClaimDto> Result { get; set; } = new();
}
