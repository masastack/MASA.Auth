namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class GetThirdPartyUserItemsRequest : PaginationRequest
{
    public string Search { get; set; }

    public bool Enabled { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public GetThirdPartyUserItemsRequest(int pageIndex, int pageSize, string search, bool enabled, Guid thirdPartyIdpId)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
        ThirdPartyIdpId = thirdPartyIdpId;
    }
}

