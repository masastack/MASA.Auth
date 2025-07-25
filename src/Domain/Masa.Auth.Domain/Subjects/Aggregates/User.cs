// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Aggregates;

public class User : FullAggregateRoot<Guid, Guid>
{
    private List<UserRole> _roles = new();
    private List<UserClaimValue> _userClaims = new();
    private List<UserPermission> _permissions = new();
    private List<ThirdPartyUser> _thirdPartyUsers = new();
    private List<UserSystemBusinessData> _systemBusinessDatas = new();
    private Staff? _staff;

    private string _name = "";
    private string _displayName = "";
    private string _avatar = "";
    private string _idCard = "";
    private string _account = "";
    private string _password = "";
    private PasswordType _passwordType = default!;
    private string _companyName = "";
    private string _department = "";
    private string _position = "";
    private string _phoneNumber = "";
    private string _landline = "";
    private string _email = "";
    private GenderTypes _gender;
    private AddressValue _address = new();
    private string _clientId = "";

    [AllowNull]
    public string Name
    {
        get => _name;
        private set => _name = value ?? "";
    }

    public string DisplayName
    {
        get => _displayName;
        private set => _displayName = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(DisplayName));
    }

    public string Avatar
    {
        get => _avatar;
        private set => _avatar = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(Avatar));
    }

    [AllowNull]
    public string IdCard
    {
        get => _idCard;
        private set => _idCard = value ?? "";
    }

    public string Account
    {
        get => _account;
        private set => _account = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(Account));
    }

    [AllowNull]
    public string Password
    {
        get => _password;
        private set
        {
            if (string.IsNullOrEmpty(value) is false)
                _password = _passwordType.EncryptPassword(this, value);
            else _password = "";
        }
    }

    [AllowNull]
    public PasswordType PasswordType
    {
        get => _passwordType;
        private set => _passwordType = value ?? PasswordType.MD5;
    }

    [AllowNull]
    public string CompanyName
    {
        get => _companyName;
        private set => _companyName = value ?? "";
    }

    [AllowNull]
    public string Department
    {
        get => _department;
        private set => _department = value ?? "";
    }

    [AllowNull]
    public string Position
    {
        get => _position;
        private set => _position = value ?? "";
    }

    public bool Enabled { get; private set; } = true;

    public GenderTypes Gender
    {
        get => _gender;
        private set
        {
            _gender = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(Gender));
        }
    }

    /// <summary>
    /// Client ID, used to record which client the user registered from
    /// </summary>
    [AllowNull]
    public string ClientId
    {
        get => _clientId;
        private set => _clientId = value ?? "";
    }

    #region Contact Property

    [AllowNull]
    public string PhoneNumber
    {
        get => _phoneNumber;
        private set => _phoneNumber = value ?? "";
    }

    [AllowNull]
    public string Landline
    {
        get => _landline;
        private set => _landline = value ?? "";
    }

    [AllowNull]
    public string Email
    {
        get => _email;
        private set => _email = value ?? "";
    }

    [AllowNull]
    public AddressValue Address
    {
        get => _address;
        private set
        {
            _address = value ?? new();
        }
    }

    #endregion

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public IReadOnlyCollection<UserPermission> Permissions => _permissions;

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

    public IReadOnlyCollection<UserClaimValue> UserClaims => _userClaims;

    public IReadOnlyCollection<UserSystemBusinessData> SystemBusinessDatas => _systemBusinessDatas;

    public Staff? Staff => _staff;

    private User()
    { }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string? email,
                string phoneNumber,
                PasswordType? passwordType = null,
                string? clientId = null) :
        this(name, displayName, avatar, default, account, password, companyName, default, default, phoneNumber, default, email, GenderTypes.Male, passwordType, clientId)
    {
    }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string? email,
                string phoneNumber,
                ThirdPartyUser thirdPartyUser,
                PasswordType? passwordType = null,
                string? clientId = null) :
        this(name, displayName, avatar, account, password, companyName, email, phoneNumber, passwordType, clientId)
    {
        _thirdPartyUsers.Add(thirdPartyUser);
    }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string? email,
                string phoneNumber,
                ThirdPartyUser thirdPartyUser,
                Staff staff,
                PasswordType? passwordType = null,
                string? clientId = null) :
        this(name, displayName, avatar, account, password, companyName, email, phoneNumber, thirdPartyUser, passwordType, clientId)
    {
        _staff = staff;
    }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string? email,
                string phoneNumber,
                Staff staff,
                PasswordType? passwordType = null,
                string? clientId = null) :
        this(name, displayName, avatar, account, password, companyName, email, phoneNumber, passwordType, clientId)
    {
        _staff = staff;
    }

    public User(Guid id,
                string? name,
                string? displayName,
                string? avatar,
                string? idCard,
                string? account,
                string? password,
                string? companyName,
                string? department,
                string? position,
                string? phoneNumber,
                string? landline,
                string? email,
                AddressValue? address,
                GenderTypes gender,
                PasswordType? passwordType = null,
                string? clientId = null)
    {
        Id = id;
        Name = name;
        IdCard = idCard;
        CompanyName = companyName;
        Department = department;
        Position = position;
        Address = address;
        Landline = landline;
        Gender = gender == default ? GenderTypes.Male : gender;
        Avatar = string.IsNullOrEmpty(avatar) ? DefaultUserAttributes.GetDefaultAvatar(Gender) : avatar;
        PasswordType = passwordType ?? PasswordType.MD5;
        Password = password;
        ClientId = clientId;
        var value = VerifyPhonNumberEmail(phoneNumber, email);
        Account = string.IsNullOrEmpty(account) ? value : account;
        DisplayName = string.IsNullOrEmpty(displayName) ? value : displayName;
    }

    public User(string? name,
                string? displayName,
                string? avatar,
                string? idCard,
                string? account,
                string? password,
                string? companyName,
                string? department,
                string? position,
                string phoneNumber,
                string? landline,
                string? email,
                GenderTypes gender,
                PasswordType? passwordType = null,
                string? clientId = null)
        : this(default,
               name,
               displayName,
               avatar,
               idCard,
               account,
               password,
               companyName,
               department,
               position,
               phoneNumber,
               landline,
               email,
               new(),
               gender,
               passwordType,
               clientId)
    {
    }

    [return: NotNullIfNotNull("user")]
    public static implicit operator UserDetailDto?(User? user)
    {
        if (user is null) return null;
        var roles = user.Roles.Select(ur => new RoleModel
        {
            Id = ur.RoleId,
            Code = ur.Role?.Code ?? "",
            Name = ur.Role?.Name ?? ""
        }).ToList();
        var permissions = user.Permissions.Select(p => new SubjectPermissionRelationDto(p.PermissionId, p.Effect)).ToList();
        var thirdPartyIdpAvatars = user.ThirdPartyUsers.Select(tpu => tpu.IdentityProvider.Icon).ToList();
        return new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, thirdPartyIdpAvatars, "", "", user.ModificationTime, user.Department, user.Position, user.Password, user.Gender, roles, permissions, user.Landline, user.ClientId);
    }

    public void Update(string account, string? name, string displayName, string avatar, string? idCard, string? companyName, string? phoneNumber, string? landline, string? email, AddressValue address, string? department, string? position, GenderTypes gender)
    {
        Account = account;
        Name = name;
        IdCard = idCard;
        CompanyName = companyName;
        Address = address;
        Department = department;
        Position = position;
        Gender = gender;
        Landline = landline;
        DisplayName = displayName;
        UpdateAvatar(avatar);
        VerifyPhonNumberEmail(phoneNumber, email);
    }

    public void Update(string? name, string displayName, string? idCard, string? companyName, string? department, GenderTypes gender)
    {
        Name = name;
        DisplayName = displayName;
        IdCard = idCard;
        CompanyName = companyName;
        Department = department;
        Gender = gender;
    }

    public void UpdateBasicInfo(string name, string displayName, GenderTypes gender, string? avatar, string? companyName, string? department, string? position, AddressValue address)
    {
        if (!string.IsNullOrEmpty(avatar)) Avatar = avatar.ToString();
        Name = name;
        DisplayName = displayName;
        Gender = gender;
        CompanyName = companyName;
        Department = department;
        Position = position;
        Address = address;
    }

    public void UpdateAvatar(string avatar)
    {
        Avatar = avatar;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Update client ID
    /// </summary>
    /// <param name="clientId">Client ID</param>
    public void UpdateClientId(string? clientId)
    {
        ClientId = clientId;
    }

    public void Disable()
    {
        Enabled = false;
        Staff?.Disable();
        _thirdPartyUsers.ForEach(thirdUser =>
        {
            thirdUser.Disable();
        });
    }

    public void Enable()
    {
        Enabled = true;
        _thirdPartyUsers.ForEach(thirdUser =>
        {
            thirdUser.Enable();
        });
    }

    [MemberNotNull(nameof(Password))]
    public void UpdatePassword(string? password)
    {
        Password = password;
    }

    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(Password) && string.IsNullOrWhiteSpace(password))
            return true;
        return _passwordType.VerifyPassword(this, Password, password ?? "");
    }

    public void AddRoles(IEnumerable<Guid> roleIds)
    {
        _roles = _roles.MergeDistinctBy(roleIds.Select(roleId => new UserRole(roleId)), item => item.RoleId).ToList();
    }

    public void SetUserClaims(IEnumerable<KeyValuePair<string, string>> claimValues)
    {
        _userClaims = claimValues.Select(claimValue => new UserClaimValue(claimValue.Key, claimValue.Value)).ToList();
    }

    public void SetRoles(IEnumerable<Guid> roleIds)
    {
        _roles = roleIds.Select(roleId => new UserRole(roleId)).ToList();
    }

    public void RemoveRoles(IEnumerable<Guid> roleIds)
    {
        _roles = _roles.Where(role => roleIds.Any(roleId => role.RoleId == roleId) is false)
                       .ToList();
    }

    public void AddPermissions(List<SubjectPermissionRelationDto> permissions)
    {
        _permissions = _permissions.MergeBy(
           permissions.Select(spr => new UserPermission(spr.PermissionId, spr.Effect)),
           item => item.PermissionId,
           (oldValue, newValue) =>
           {
               oldValue.Update(newValue.Effect);
               return oldValue;
           });
    }

    public void Bind(Staff staff)
    {
        _staff = staff;
    }

    public bool IsAdmin()
    {
        return Account == "admin";
    }

    [MemberNotNull(nameof(PhoneNumber))]
    [MemberNotNull(nameof(Email))]
    string VerifyPhonNumberEmail(string? phoneNumber, string? email)
    {
        if (string.IsNullOrEmpty(phoneNumber) && string.IsNullOrEmpty(email))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PHONE_NUMBER_AND_EMAIL_EMPTY_ERROR);
        }
        PhoneNumber = phoneNumber;
        Email = email;
        return string.IsNullOrEmpty(PhoneNumber) ? Email : PhoneNumber;
    }
}
