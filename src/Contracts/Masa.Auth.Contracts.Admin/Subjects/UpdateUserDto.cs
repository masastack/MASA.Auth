namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserDto
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public AddressValueDto Address { get; set; }

    public UpdateUserDto(Guid userId, string name, string displayName, string avatar, string companyName, bool enabled, AddressValueDto address)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        CompanyName = companyName;
        Enabled = enabled;
        Address = address;
    }

    public static implicit operator UpdateUserDto(UserDetailDto user)
    {
        return new UpdateUserDto(user.UserId, user.Name, user.DisplayName, user.Avatar, user.CompanyName, user.Enabled, user.Address);
    }
}
