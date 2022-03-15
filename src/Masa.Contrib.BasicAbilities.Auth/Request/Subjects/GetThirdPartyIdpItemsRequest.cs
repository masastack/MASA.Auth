namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class GetThirdPartyIdpItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public GetThirdPartyIdpItemsRequest(int pageIndex, int pageSize, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

