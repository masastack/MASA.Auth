namespace Masa.Auth.Sdk.Request.Subjects;

public class GetStaffItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public GetStaffItemsRequest(int pageIndex, int pageSize, string search, bool enabled)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }
}

