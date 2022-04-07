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

    private List<UserRole> userRoles = new();

    public IReadOnlyCollection<UserRole> UserRoles => userRoles;

    #endregion

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, string department, string position, bool enabled, string phoneNumber, string email, AddressValue address)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        Password = password;
        CompanyName = companyName;
        Department = department;
        Position = position;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
    }

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, string department, string position, bool enabled, string phoneNumber, string email) : this(name, displayName, avatar, idCard, account, password, companyName, department, position, enabled, phoneNumber, email, new())
    {

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

    public static implicit operator UserDetailDto(User user)
    {
        return new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, new(), "", "", user.ModificationTime, user.Department, user.Position, user.Password);
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