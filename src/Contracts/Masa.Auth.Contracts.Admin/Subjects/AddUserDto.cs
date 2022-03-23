namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddUserDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IdCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public AddressValueDto Address { get; set; }

    public AddUserDto(string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }

    public static implicit operator AddUserDto(UserDetailDto user)
    {
        return new AddUserDto(user.Name, user.DisplayName, user.Avatar, user.IdCard, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.Address);
    }
}
