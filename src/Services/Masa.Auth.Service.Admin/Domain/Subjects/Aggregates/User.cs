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

    public bool Enabled { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public AddressValue Address { get; private set; }

    private List<UserRole> userRoles = new();

    public IReadOnlyCollection<UserRole> UserRoles => userRoles;

    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password,
        string companyName, bool enabled, string phoneNumber, string email,
        AddressValue address)
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
        Address = address;
    }

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, bool enabled, string phoneNumber, string email)
        : this(name, displayName, avatar, idCard, account, password, companyName, enabled, phoneNumber, email, new AddressValue())
    {
    }

    public void Update()
    {

    }

    public void AddRole(params Guid[] roleIds)
    {
        userRoles.AddRange(roleIds.Select(roleId => new UserRole(roleId)));
    }

    public void RemoveRole(params Guid[] roleIds)
    {
        userRoles.RemoveAll(ur => roleIds.Contains(ur.RoleId));
    }
}