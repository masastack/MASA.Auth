namespace Masa.Auth.Sdk.Request.Subjects;

public class GetThirdPartyUserItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public Guid ThirdPartyPlatformId { get; set; }

    public GetThirdPartyUserItemsRequest(int pageIndex, int pageSize, string search, bool enabled, Guid thirdPartyPlatformId)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
        ThirdPartyPlatformId = thirdPartyPlatformId;
    }
}

