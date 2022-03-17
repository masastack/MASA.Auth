namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class GetThirdPartyIdpItemsRequest : PaginationRequest
{
    public string Search { get; set; }

    public GetThirdPartyIdpItemsRequest(int pageIndex, int pageSize, string search)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
    }
}

