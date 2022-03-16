namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class AddUserRequest
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IDCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public AddressValue Address { get; set; }

    public AddUserRequest(string name, string displayName, string avatar, string iDCard, string companyName, bool enabled, string phoneNumber, string email, AddressValue address)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = iDCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }

    public static implicit operator AddUserRequest(UserDetailResponse user)
    {
        return new AddUserRequest(user.Name, user.DisplayName, user.Avatar, user.IDCard, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.Address);
    }
}
