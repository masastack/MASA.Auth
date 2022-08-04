// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class User : FullAggregateRoot<Guid, Guid>
{
    private List<UserRole> _roles = new();
    private List<SubjectPermissionRelation> _permissions = new();
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

    public IReadOnlyCollection<SubjectPermissionRelation> Permissions => _permissions;

    public IReadOnlyCollection<ThirdPartyUser> ThirdPartyUsers => _thirdPartyUsers;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    private User()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {

    }

    public User(string name,
                string displayName,
                string avatar,
                string account,
                string password,
                string companyName) :
        this(name, displayName, avatar, "", account, password, companyName, "", "", true, "", "", "", GenderTypes.Male)
    {
    }

    public User(string name,
                string displayName,
                string avatar,
                string idCard,
                string account,
                string password,
                string companyName,
                string department,
                string position,
                bool enabled,
                string phoneNumber,
                string landline,
                string email,
                AddressValue address,
                GenderTypes genderType)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        UpdatePassword(password);
        CompanyName = companyName;
        Department = department;
        Position = position;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        GenderType = genderType;
        Landline = landline;
    }

    public User(string name,
                string displayName,
                string avatar,
                string idCard,
                string account,
                string password,
                string companyName,
                string department,
                string position,
                bool enabled,
                string phoneNumber,
                string landline,
                string email,
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

    public static implicit operator UserDetailDto(User user)
    {
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
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        GenderType = genderType;
        Landline = landline;
    }

    public void Update(string name, string? displayName, string? idCard, string? phoneNumber, string landline, string? email, string? position, GenderTypes genderType)
    {
        Name = name;
        DisplayName = displayName ?? "";
        IdCard = idCard ?? "";
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        Position = position ?? "";
        GenderType = genderType;
        Landline = landline;
    }

    public void Update(string name, string? displayName, string? idCard, string? companyName, string? phoneNumber, string? email, GenderTypes genderType)
    {
        Name = name;
        DisplayName = displayName ?? "";
        IdCard = idCard ?? "";
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
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
    public void UpdatePassword(string password)
    {
        if (password is null) throw new UserFriendlyException("Password cannot be null");

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

    public bool IsAdmin()
    {
        return Account == "admin";
    }

    public void AddPermissions(List<SubjectPermissionRelationDto> permissions)
    {
        _permissions = _permissions.MergeBy(
           permissions.Select(spr => new SubjectPermissionRelation(spr.PermissionId, PermissionRelationTypes.UserPermission, spr.Effect)),
           item => item.PermissionId,
           (oldValue, newValue) =>
           {
               oldValue.Update(newValue.Effect);
               return oldValue;
           });
    }
}