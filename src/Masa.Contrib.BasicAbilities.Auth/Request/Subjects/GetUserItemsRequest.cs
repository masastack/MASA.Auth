namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class GetUserItemsRequest : PaginationRequest
{
    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public bool Enabled { get; set; }

    public GetUserItemsRequest(int pageIndex, int pageSize, string name, string phoneNumber, string email, bool enabled)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Enabled = enabled;
    }
}

