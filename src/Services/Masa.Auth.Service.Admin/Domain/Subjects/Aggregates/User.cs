// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class User : FullAggregateRoot<Guid, Guid>
{
    private List<UserRole> _roles = new();
    private List<UserPermission> _permissions = new();
    private List<ThirdPartyUser> _thirdPartyUsers = new();

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

    public string Landline { get; private set; }

    public string Email { get; private set; }

    public AddressValue Address { get; private set; }

    #endregion

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public IReadOnlyCollection<UserPermission> Permissions => _permissions;

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null ���ֶα�������� null ֵ���뿼������Ϊ����Ϊ null��
    private User()
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null ���ֶα�������� null ֵ���뿼������Ϊ����Ϊ null��
    {

    }

    public User(string? name,
                string displayName,
                string? avatar,
                string? account,
                string? password,
                string? companyName,
                string phoneNumber) :
        this(name, displayName, avatar, default, account, password, companyName, default, default, true, phoneNumber, default, default, GenderTypes.Male)
    {
    }

    public User(string? name,
                string displayName,
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
                AddressValue address,
                GenderTypes genderType)
    {
        Name = name ?? "";
        DisplayName = displayName;
        Avatar = avatar ?? "";
        IdCard = idCard ?? "";
        CompanyName = companyName ?? "";
        Department = department ?? "";
        Position = position ?? "";
        Enabled = enabled;
        Address = address;
        GenderType = genderType;
        Landline = landline ?? "";
        UpdatePassword(password);
        var value = VerifyPhonNumberEmail(phoneNumber, email);
        Account = string.IsNullOrEmpty(account) ? value : account;
    }

    public User(string? name,
                string displayName,
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
                GenderTypes genderType)
        : this(name,
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
               genderType)
    {
    }

    [return: NotNullIfNotNull("user")]
    public static implicit operator UserDetailDto?(User? user)
    {
        if (user is null) return null;
        var roles = user.Roles.Select(r => r.RoleId).ToList();
        var permissions = user.Permissions.Select(p => new SubjectPermissionRelationDto(p.PermissionId, p.Effect)).ToList();
        var thirdPartyIdpAvatars = user.ThirdPartyUsers.Select(tpu => tpu.IdentityProvider.Icon).ToList();
        return new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, thirdPartyIdpAvatars, "", "", user.ModificationTime, user.Department, user.Position, user.Password, user.GenderType, roles, permissions);
    }

    public void Update(string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string landline, string email, AddressValueDto address, string department, string position, GenderTypes genderType)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        VerifyPhonNumberEmail(phoneNumber, email);
        Address = address;
        Department = department;
        Position = position;
        GenderType = genderType;
        Landline = landline;
    }

    public void Update(string name, string? displayName, string? idCard, string? companyName, string? phoneNumber, string? email, GenderTypes genderType)
    {
        Name = name;
        DisplayName = displayName ?? "";
        IdCard = idCard ?? "";
        VerifyPhonNumberEmail(phoneNumber, email);
        GenderType = genderType;
        CompanyName = companyName ?? "";
    }

    public void UpdateBasicInfo(string? displayName, string? phoneNumber, string? email, string avatar, GenderTypes genderType)
    {
        DisplayName = displayName ?? "";
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        GenderType = genderType;
        Avatar = avatar;
    }

    public void Disabled()
    {
        Enabled = false;
    }

    [MemberNotNull(nameof(Password))]
    public void UpdatePassword(string? password)
    {
        if (password is null) password = "";

        Password = MD5Utils.EncryptRepeat(password);
    }

    public bool VerifyPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && Password == MD5Utils.EncryptRepeat(password);
    }

    public void AddRoles(params Guid[] roleIds)
    {
        _roles = _roles.MergeBy(
           roleIds.Select(roleId => new UserRole(roleId)),
           item => item.RoleId);
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
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        return string.IsNullOrEmpty(PhoneNumber) ? Email : PhoneNumber;
    }
}