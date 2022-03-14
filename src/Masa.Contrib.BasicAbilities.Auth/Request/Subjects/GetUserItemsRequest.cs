namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class GetUserItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public GetUserItemsRequest(int pageIndex, int pageSize, string search, bool enabled)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }
}

