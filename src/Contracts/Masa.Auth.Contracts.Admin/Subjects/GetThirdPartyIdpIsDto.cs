namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyIdpIsDto : Pagination<GetThirdPartyIdpIsDto>
{
    public string Search { get; set; }

    public GetThirdPartyIdpIsDto(int page, int pageSize, string search)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
    }
}

