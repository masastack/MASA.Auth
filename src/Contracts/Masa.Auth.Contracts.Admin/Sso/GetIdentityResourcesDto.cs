namespace Masa.Auth.Contracts.Admin.Sso;

public class GetIdentityResourcesDto : Pagination<GetIdentityResourcesDto>
{
    public string Search { get; set; }

    public GetIdentityResourcesDto(string search)
    {
        Search = search;
    }

    public GetIdentityResourcesDto(int page, int pageSize, string search)
    {
        Search = search;
        Page = page;
        PageSize = pageSize;
    }
}

