namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDetailDto : UserDto
{
    public AddressValueDto Address { get; set; }

    public UserDetailDto() : base()
    {
        Address = new();
    }

    public UserDetailDto(Guid id, string name, string displayName, string avatar, string idCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, AddressValueDto address) : base(id, name, displayName, avatar, idCard, account, companyName, enabled, phoneNumber, email, creationTime)
    {
        Address = address;
    }
}

