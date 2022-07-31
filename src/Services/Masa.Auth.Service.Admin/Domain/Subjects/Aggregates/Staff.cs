// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : FullAggregateRoot<Guid, Guid>
{
    private User? _user;
    private Position? _position;
    private List<DepartmentStaff> _departmentStaffs = new();
    private List<TeamStaff> _teamStaffs = new();
    private Guid? _positionId;
    private User? _createUser;
    private User? _modifyUser;

    public virtual User User => _user ?? throw new UserFriendlyException("Failed to get user data");

    public virtual Position? Position => _position;

    public virtual IReadOnlyList<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public virtual IReadOnlyList<TeamStaff> TeamStaffs => _teamStaffs;

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Avatar { get; private set; } = "";

    public string IdCard { get; private set; } = "";

    public string Account { get; private set; } = "";

    public string Password { get; private set; } = "";

    public string CompanyName { get; private set; } = "";

    public GenderTypes Gender { get; private set; }

    #region Contact Property

    public string PhoneNumber { get; private set; } = "";

    public string Email { get; private set; } = "";

    public AddressValue Address { get; private set; } = new();

    #endregion

    public string JobNumber { get; private set; } = "";

    public Guid? PositionId
    {
        get => _positionId;
        set
        {
            if (value == Guid.Empty) _positionId = null;
            else _positionId = value;
        }
    }

    public StaffTypes StaffType { get; private set; }

    public bool Enabled { get; private set; }

    public Staff(Guid userId, string name, string displayName, string avatar, string idCard, string account, string password, string companyName, GenderTypes gender, string? phoneNumber, string? email, AddressValue address, string jobNumber, Guid? positionId, StaffTypes staffType, bool enabled)
    {
        UserId = userId;
        Name = name ?? "";
        DisplayName = displayName ?? "";
        Avatar = avatar ?? "";
        IdCard = idCard ?? "";
        Account = account ?? "";
        Password = password ?? throw new UserFriendlyException("Password cannot be empty");
        CompanyName = companyName ?? "";
        Gender = gender;
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        Address = address ?? new();
        JobNumber = jobNumber ?? "";
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public Staff(Guid userId, string name, string displayName, string avatar, string idCard, string account, string password, string companyName, GenderTypes gender, string? phoneNumber, string? email, string jobNumber, Guid? positionId, StaffTypes staffType, bool enabled)
    {
        UserId = userId;
        Name = name ?? "";
        DisplayName = displayName ?? "";
        Avatar = avatar ?? "";
        IdCard = idCard ?? "";
        Account = account ?? "";
        UpdatePassword(password);
        CompanyName = companyName ?? "";
        Gender = gender;
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        JobNumber = jobNumber ?? "";
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public static implicit operator StaffDetailDto(Staff staff)
    {
        var teams = staff.TeamStaffs.Select(t => t.TeamId).ToList();
        var departmentStaff = staff.DepartmentStaffs.FirstOrDefault();
        UserDetailDto user = staff.User;
        return new StaffDetailDto(departmentStaff?.DepartmentId ?? Guid.Empty, staff.PositionId ?? Guid.Empty, teams, staff.Password, user.ThirdPartyIdpAvatars, staff.CreateUser?.Name ?? "", staff.ModifyUser?.Name ?? "", staff.ModificationTime, user.RoleIds, user.Permissions, staff.Id, staff.UserId, "", staff.Position?.Name ?? "", staff.JobNumber, staff.Enabled, staff.StaffType, staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.Account, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.CreationTime, staff.Gender);
    }

    public static implicit operator StaffDto(Staff staff)
    {
        var department = staff.DepartmentStaffs.FirstOrDefault()?.Department?.Name ?? ""; ;
        return new StaffDto(staff.Id, staff.UserId, department, staff.Position?.Name ?? "", staff.JobNumber, staff.Enabled, staff.StaffType, staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.Account, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.CreationTime, staff.Gender);
    }

    public void Update(Guid? positionId, StaffTypes staffType, bool enabled, string? name, string? displayName, string? avatar, string? idCard, string? companyName, string? phoneNumber, string? email, AddressValueDto? address, GenderTypes gender)
    {
        Name = name ?? "";
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
        Name = name ?? "";
        DisplayName = displayName ?? "";
        IdCard = idCard ?? "";
        PhoneNumber = phoneNumber ?? "";
        Email = email ?? "";
        Gender = gender;
        Avatar = avatar ?? "";
        CompanyName = companyName ?? "";
        Enabled = enabled;
        Address = address ?? new();
    }

    [MemberNotNull(nameof(Password))]
    public void UpdatePassword(string password)
    {
        if (password is null) throw new UserFriendlyException("Password cannot be null");

        Password = MD5Utils.EncryptRepeat(password);
    }

    public void SetDepartmentStaff(Guid departmentId)
    {
        _departmentStaffs = _departmentStaffs.MergeBy(
            new[] { new DepartmentStaff(departmentId, default) },
            item => item.DepartmentId);
    }

    public void SetTeamStaff(List<Guid> teams)
    {
        _teamStaffs = _teamStaffs.MergeBy(
            teams.Select(teamId => new TeamStaff(teamId, default, TeamMemberTypes.Member, UserId)),
            team => team.TeamId);
    }
}
