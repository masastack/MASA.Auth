namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string Avatar { get; private set; }

    public string IdCard { get; private set; }

    public string Account { get; private set; }

    public string Password { get; private set; }

    public string CompanyName { get; private set; }

    public bool Enabled { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public AddressValue Household { get; private set; }

    public AddressValue Residential { get; private set; }

    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password,
        string companyName, bool enabled, string phoneNumber, string email,
        AddressValue householdAddress, AddressValue residentialAddress)
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
        Household = householdAddress;
        Residential = residentialAddress;
    }

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, bool enabled, string phoneNumber, string email)
        : this(name, displayName, avatar, idCard, account, password, companyName, enabled, phoneNumber, email, new AddressValue(), new AddressValue())
    {
    }

    public void Update()
    {

    }
}