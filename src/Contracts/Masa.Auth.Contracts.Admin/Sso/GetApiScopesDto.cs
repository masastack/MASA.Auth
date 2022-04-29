namespace Masa.Auth.Contracts.Admin.Sso;

public class GetApiScopesDto : Pagination<GetApiScopesDto>
{
    public string Search { get; set; }

    public GetApiScopesDto(int page, int pageSize, string search)
    {
        Search = search;
        Page = page;
        PageSize = pageSize;
    }
}

