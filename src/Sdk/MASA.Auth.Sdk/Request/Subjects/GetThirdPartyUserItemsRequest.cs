namespace Masa.Auth.Sdk.Request.Subjects;

public class GetThirdPartyUserItemsRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public UserStates UserState { get; set; }

    public GetThirdPartyUserItemsRequest(int pageIndex, int pageSize, string search, UserStates userState)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        UserState = userState;
    }
}

