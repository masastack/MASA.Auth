namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Avatar { get; private set; }

    public string PhoneNumber { get; private set; }

    public string IDNumber { get; private set; }

    public string Email { get; private set; }

    public string Account { get; private set; }

    public string Password { get; private set; }

    public User(string name, string displayName, string avatar, string phoneNumber, string iDNumber, string email, string account, string password)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        IDNumber = iDNumber;
        Email = email;
        Account = account;
        Password = password;
    }
}

