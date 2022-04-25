namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IdCard { get; set; }

    public string Account { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public DateTime CreationTime { get; set; }

    public GenderTypes Gender { get; set; }

    public UserDto()
    {
        Name = "";
        DisplayName = "";
        Avatar = "";
        IdCard = "";
        Account = "";
        CompanyName = "";
        PhoneNumber = "";
        Email = "";
    }

    public UserDto(Guid id, string name, string displayName, string avatar, string idCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, GenderTypes genderType)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        CreationTime = creationTime;
        Gender = genderType;
    }
}

