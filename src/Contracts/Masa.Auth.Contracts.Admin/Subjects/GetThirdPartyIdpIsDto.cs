namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyIdpIsDto : Pagination
{
    public string Search { get; set; }

    public GetThirdPartyIdpIsDto(int pageIndex, int pageSize, string search)
    {
        Page = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

