namespace Masa.Auth.Contracts.Admin.Sso;

public class GetUserClaimsDto : Pagination<GetUserClaimsDto>
{
    public string Search { get; set; }

    public GetUserClaimsDto(int page, int pageSize, string search)
    {
        Search = search;
        Page = page;
        PageSize = pageSize;
    }
}

