namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class UserDetailResponse : UserItemResponse
{ 
    public AddressValue Address { get; set; }

    public static UserDetailResponse Default => new UserDetailResponse(Guid.Empty, "", "", "", "", "", "", default, "", "", DateTime.Now, new());

    public UserDetailResponse(Guid userId, string name, string displayName, string avatar, string iDCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, AddressValue address) : base(userId, name, displayName, avatar, iDCard, account, companyName, enabled, phoneNumber, email, creationTime)
    {
        Address = address;
    }
}

