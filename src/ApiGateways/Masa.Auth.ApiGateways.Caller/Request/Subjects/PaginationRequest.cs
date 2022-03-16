namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class PaginationRequest
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

