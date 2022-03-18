namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyIdpIsDto : Pagination
{
    public string Search { get; set; }

    public GetThirdPartyIdpIsDto(int pageIndex, int pageSize, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

