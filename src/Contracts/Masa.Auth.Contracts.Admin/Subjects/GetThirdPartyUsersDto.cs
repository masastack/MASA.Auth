namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyUsersDto : Pagination
{
    public string Search { get; set; }

    public bool Enabled { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public GetThirdPartyUsersDto(int page, int pageSize, string search, bool enabled, Guid thirdPartyIdpId)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
        ThirdPartyIdpId = thirdPartyIdpId;
    }
}

