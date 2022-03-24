namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Avatar { get; private set; }

    public string IdCard { get; private set; }

    public string Account { get; private set; }

    public string Password { get; private set; }

    public string CompanyName { get; private set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public bool Enabled { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public AddressValue Address { get; private set; }

    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password,
        string companyName, bool enabled, string phoneNumber, string email,
        string department,string position, AddressValue? address = null)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        Password = password;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address ?? new();
        Department = department;
        Position = position;
    }

    public void Update(string name, string displayName, string avatar, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address, string department, string position, string password)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Password = password;
    }
}