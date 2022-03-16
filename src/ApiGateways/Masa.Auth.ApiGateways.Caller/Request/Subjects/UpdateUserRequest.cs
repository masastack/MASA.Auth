namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class UpdateUserRequest
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public AddressValue Address { get; set; }

    public UpdateUserRequest(Guid userId, string name, string displayName, string avatar, string companyName, bool enabled, AddressValue address)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        CompanyName = companyName;
        Enabled = enabled;
        Address = address;
    }

    public static implicit operator UpdateUserRequest(UserDetailResponse user)
    {
        return new UpdateUserRequest(user.UserId, user.Name, user.DisplayName, user.Avatar, user.CompanyName, user.Enabled, user.Address);
    }
}
