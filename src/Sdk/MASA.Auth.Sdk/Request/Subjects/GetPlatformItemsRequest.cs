namespace Masa.Auth.Sdk.Request.Subjects;

public class GetPlatformItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public GetPlatformItemsRequest(int pageIndex, int pageSize, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

