namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class GetThirdPartyPlatformItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public GetThirdPartyPlatformItemsRequest(int pageIndex, int pageSize, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

