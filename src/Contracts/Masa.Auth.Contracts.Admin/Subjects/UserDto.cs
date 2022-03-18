namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDto
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IDCard { get; set; }

    public string Account { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public DateTime CreationTime { get; set; }

    public UserDto(Guid userId, string name, string displayName, string avatar, string iDCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IDCard = iDCard;
        Account = account;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        CreationTime = creationTime;
    }
}

