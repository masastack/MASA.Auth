namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserDto
{
    public Guid Id { get; set; }

    [RegularExpression("^[\u4e00-\u9fa5a-zA-Z-z]+$")]
    [MinLength(1)]
    [MaxLength(20)]
    public string Name { get; set; }

    [Required]
    [RegularExpression("^[\u4e00-\u9fa5a-zA-Z-z]+$")]
    [MinLength(1)]
    [MaxLength(20)]
    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    [RegularExpression("(^\\d{18}$)|(^\\d{15}$)", ErrorMessage = "IdCard format is incorrect")]
    public string IdCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string Password { get; set; }

    public UpdateUserDto(Guid id, string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address, string department, string position, string password)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Password = password;
    }

    public static implicit operator UpdateUserDto(UserDetailDto user)
    {
        return new UpdateUserDto(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard,user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.Address, user.Department, user.Position, user.Password);
    }
}
