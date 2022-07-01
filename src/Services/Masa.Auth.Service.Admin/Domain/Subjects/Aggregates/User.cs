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
        var permissions = user.Permissions.Select(p => new UserPermissionDto(p.PermissionId, p.Effect)).ToList();
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