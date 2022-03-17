namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class GetTeamItemsRequest : PaginationRequest
{
    public GetTeamItemsRequest(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}

