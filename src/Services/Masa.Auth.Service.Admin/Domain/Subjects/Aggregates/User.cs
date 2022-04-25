namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class User : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    private List<UserRole> _roles = new();
    private List<UserPermission> _permissions = new();
    private List<ThirdPartyUser> _thirdPartyUsers = new();

    public bool IsDeleted { get; private set; }

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

    public GenderTypes GenderType { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; }

    public string Email { get; private set; }

    public AddressValue Address { get; private set; }

    #endregion

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public IReadOnlyCollection<UserPermission> Permissions => _permissions;

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, string department, string position, bool enabled, string phoneNumber, string email, AddressValue address, GenderTypes genderType)
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
        GenderType = genderType;
    }

    public User(string name, string displayName, string avatar, string idCard, string account, string password, string companyName, string department, string position, bool enabled, string phoneNumber, string email, GenderTypes genderType) : this(name, displayName, avatar, idCard, account, password, companyName, department, position, enabled, phoneNumber, email, new(), genderType)
    {

    }

    public void Update(string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address, string department, string position, string password, GenderTypes genderType)
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
        Department = department;
        Position = position;
        Password = password;
        GenderType = genderType;
    }

    public static implicit operator UserDetailDto(User user)
    {
        var roles = user.Roles.Select(r => r.RoleId).ToList();
        var permissions = user.Permissions.Select(p => new UserPermissionDto(p.PermissionId, p.Effect)).ToList();
        var thirdPartyIdpAvatars = user.ThirdPartyUsers.Select(tpu => tpu.ThirdPartyIdp.Icon).ToList();
        return new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, thirdPartyIdpAvatars, "", "", user.ModificationTime, user.Department, user.Position, user.Password, user.GenderType, roles, permissions);
    }

    public void AddRoles(params Guid[] roleIds)
    {
        _roles.Clear();
        _roles.AddRange(roleIds.Select(roleId => new UserRole(roleId)));
    }

    public void RemoveRoles(params Guid[] roleIds)
    {
        _roles.RemoveAll(ur => roleIds.Contains(ur.RoleId));
    }

    public void AddPermissions(List<UserPermission> permissions)
    {
        _permissions.Clear();
        _permissions.AddRange(permissions);
    }
}