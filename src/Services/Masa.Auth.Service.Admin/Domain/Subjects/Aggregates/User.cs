// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class User : FullAggregateRoot<Guid, Guid>
{
    private List<UserRole> _roles = new();
    private List<UserPermission> _permissions = new();
    private List<ThirdPartyUser> _thirdPartyUsers = new();

    private string _name = "";
    private string _displayName = "";
    private string _avatar = "";
    private string _idCard = "";
    private string _account = "";
    private string _password = "";
    private string _companyName = "";
    private string _department = "";
    private string _position = "";
    private string _phoneNumber = "";
    private string _landline = "";
    private string _email = "";
    private GenderTypes _gender;
    private AddressValue _address = new();

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
                _password = MD5Utils.EncryptRepeat(value);
            else _password = "";
        }
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

    public bool Enabled { get; private set; }

    public GenderTypes GenderType
    {
        get => _gender;
        private set
        {
            _gender = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(GenderType));
        }
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

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    private User()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {

    }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string? email,
                string phoneNumber) :
        this(name, displayName, avatar, default, account, password, companyName, default, default, true, phoneNumber, default, email, GenderTypes.Male)
    {
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
                bool enabled,
                string? phoneNumber,
                string? landline,
                string? email,
                AddressValue? address,
                GenderTypes gender)
    {
        Id = id;
        Name = name;
        IdCard = idCard;
        CompanyName = companyName;
        Department = department;
        Position = position;
        Enabled = enabled;
        Address = address;
        Landline = landline;
        GenderType = gender == default ? GenderTypes.Male : gender;
        Avatar = string.IsNullOrEmpty(avatar) ? DefaultUserAttributes.GetDefaultAvatar(GenderType) : avatar;
        Password = password;
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
                bool enabled,
                string phoneNumber,
                string? landline,
                string? email,
                GenderTypes gender)
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
               enabled,
               phoneNumber,
               landline,
               email,
               new(),
               gender)
    {
    }

    [return: NotNullIfNotNull("user")]
    public static implicit operator UserDetailDto?(User? user)
    {
        if (user is null) return null;
        var roles = user.Roles.Select(ur => new RoleModel 
        {
           Id = ur.Role.Id,
           Code = ur.Role.Code
        }).ToList();
        var permissions = user.Permissions.Select(p => new SubjectPermissionRelationDto(p.PermissionId, p.Effect)).ToList();
        var thirdPartyIdpAvatars = user.ThirdPartyUsers.Select(tpu => tpu.IdentityProvider.Icon).ToList();
        return new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, thirdPartyIdpAvatars, "", "", user.ModificationTime, user.Department, user.Position, user.Password, user.GenderType, roles, permissions, user.Landline);
    }

    public void Update(string? name, string displayName, string avatar, string? idCard, string? companyName, bool enabled, string? phoneNumber, string? landline, string? email, AddressValue address, string? department, string? position, GenderTypes gender)
    {
        Name = name;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        Address = address;
        Department = department;
        Position = position;
        GenderType = gender;
        Landline = landline;
        DisplayName = displayName;
        Avatar = avatar;
        VerifyPhonNumberEmail(phoneNumber, email);
    }

    public void Update(string? name, string displayName, string? idCard, string? companyName, string? department, GenderTypes gender)
    {
        Name = name;
        DisplayName = displayName;
        IdCard = idCard;
        CompanyName = companyName;
        GenderType = gender;
    }

    public void UpdateBasicInfo(string displayName, GenderTypes gender)
    {
        DisplayName = displayName;
        GenderType = gender;
    }

    public void UpdateAvatar(string avatar)
    {
        Avatar = avatar;
    }

    public void UpdatePhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    public void Disabled()
    {
        Enabled = false;
    }

    [MemberNotNull(nameof(Password))]
    public void UpdatePassword(string? password)
    {
        Password = password;
    }

    public bool VerifyPassword(string password)
    {
        return string.IsNullOrEmpty(Password) || Password == MD5Utils.EncryptRepeat(password);
    }

    public void AddRoles(IEnumerable<Guid> roleIds)
    {
        _roles = _roles.MergeBy(
           roleIds.Select(roleId => new UserRole(roleId)),
           item => item.RoleId).DistinctBy(ur => ur.RoleId).ToList();
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
            throw new UserFriendlyException("One of the phone number and email must be assigned");
        }
        PhoneNumber = phoneNumber;
        Email = email;
        return string.IsNullOrEmpty(PhoneNumber) ? Email : PhoneNumber;
    }
}