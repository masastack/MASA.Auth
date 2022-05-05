namespace Masa.Auth.Contracts.Admin.Sso;

public class GetCustomLoginsDto : Pagination<GetCustomLoginsDto>
{
    public string Search { get; set; }

    public GetCustomLoginsDto(int page, int pageSize, string search)
    {
        Search = search;
        Page = page;
        PageSize = pageSize;
    }
}

