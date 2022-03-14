namespace Masa.Auth.Sdk.Request.Permissions;

public class GetRoleItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public GetRoleItemsRequest(int pageIndex, int pageSize, string search, bool enabled)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }
}

