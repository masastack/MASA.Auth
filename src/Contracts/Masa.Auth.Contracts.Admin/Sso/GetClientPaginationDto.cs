namespace Masa.Auth.Contracts.Admin.Sso;

public class GetClientPaginationDto : Pagination<GetClientPaginationDto>
{
    public string Search { get; set; } = string.Empty;
}
