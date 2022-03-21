namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDetailDto : UserDto
{ 
    public AddressValueDto Address { get; set; }

    public static UserDetailDto Default => new UserDetailDto(Guid.Empty, "", "", "", "", "", "", default, "", "", DateTime.Now, new());

    public UserDetailDto(Guid id, string name, string displayName, string avatar, string iDCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, AddressValueDto address) : base(id, name, displayName, avatar, iDCard, account, companyName, enabled, phoneNumber, email, creationTime)
    {
        Address = address;
    }
}

