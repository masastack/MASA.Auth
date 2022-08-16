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

    public Staff(
        Guid userId,
        string? name,
        string? displayName,
        string? avatar,
        string? idCard,
        string? companyName,
        GenderTypes gender,
        string? phoneNumber,
        string? email,
        string jobNumber,
        Guid? positionId,
        StaffTypes staffType,
        bool enabled,
        AddressValue address)
    {
        UserId = userId;
        Name = name ?? "";
        Avatar = avatar ?? "";
        IdCard = idCard ?? "";
        CompanyName = companyName ?? "";
        Address = address ?? new();
        JobNumber = jobNumber ?? "";
        PositionId = positionId;
        Enabled = enabled;
        StaffType = staffType == default ? StaffTypes.ExternalStaff : staffType;
        Gender = gender == default ? GenderTypes.Male : gender;
        Avatar = string.IsNullOrEmpty(avatar) ? DefaultUserAttributes.GetDefaultAvatar(Gender) : avatar;
        var value = VerifyPhonNumberEmail(phoneNumber, email);
        DisplayName = string.IsNullOrEmpty(displayName) ? value : displayName;
    }

    public Staff(
        Guid userId,
        string? name,
        string displayName,
        string? avatar,
        string? idCard,
        string? companyName,
        GenderTypes gender,
        string? phoneNumber,
        string? email,
        string jobNumber,
        Guid? positionId,
        StaffTypes staffType,
        bool enabled) : this(userId, name, displayName, avatar, idCard, companyName, gender, phoneNumber, email, jobNumber, positionId, staffType, enabled, new())
    {
    }

    public static implicit operator StaffDetailDto(Staff staff)
    {
        var teams = staff.TeamStaffs.Select(t => t.TeamId).ToList();
        var departmentStaff = staff.DepartmentStaffs.FirstOrDefault();
        UserDetailDto user = staff.User;
        return new StaffDetailDto(departmentStaff?.DepartmentId ?? Guid.Empty, staff.PositionId ?? Guid.Empty, teams, user.ThirdPartyIdpAvatars, staff.CreateUser?.Name ?? "", staff.ModifyUser?.Name ?? "", staff.ModificationTime, user.RoleIds, user.Permissions, staff.Id, staff.UserId, "", staff.Position?.Name ?? "", staff.JobNumber, staff.Enabled, staff.StaffType, staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.CreationTime, staff.Gender);
    }

    public static implicit operator StaffDto(Staff staff)
    {
        var department = staff.DepartmentStaffs.FirstOrDefault()?.Department?.Name ?? ""; ;
        return new StaffDto(staff.Id, staff.UserId, department, staff.Position?.Name ?? "", staff.JobNumber, staff.Enabled, staff.StaffType, staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.CreationTime, staff.Gender);
    }

    public void Update(Guid? positionId, StaffTypes staffType, bool enabled, string? name, string displayName, string? avatar, string? idCard, string? companyName, string? phoneNumber, string? email, AddressValueDto? address, GenderTypes gender)
    {
        Name = name ?? "";
        PositionId = positionId;
        Enabled = enabled;
        Name = name ?? "";
        IdCard = idCard ?? "";
        Avatar = avatar ?? "";
        CompanyName = companyName ?? "";
        Enabled = enabled;
        Address = address ?? new();
        UpdateCore(displayName, phoneNumber, email, staffType, gender);
    }

    public void UpdateForLdap(bool enabled, string name, string displayName, string avatar, string phoneNumber, string email)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        UpdateCore(displayName, phoneNumber, email);
    }

    void UpdateCore(string displayName, string? phoneNumber, string? email, StaffTypes staffType, GenderTypes gender)
    {
        UpdateCore(displayName, phoneNumber, email);
        StaffType = ArgumentNullOrEmptyException.ThrowIfDefault(staffType);
        Gender = ArgumentNullOrEmptyException.ThrowIfDefault(gender);
    }

    void UpdateCore(string displayName, string? phoneNumber, string? email)
    {
        VerifyPhonNumberEmail(phoneNumber, email);
        DisplayName = ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(displayName);
    }

    public void SetDepartmentStaff(Guid departmentId)
    {
        if (departmentId != default)
        {
            _departmentStaffs = _departmentStaffs.MergeBy(
                new[] { new DepartmentStaff(departmentId, default) },
                item => item.DepartmentId);
        }
        else _departmentStaffs.Clear();
    }

    public void SetTeamStaff(List<Guid> teams)
    {
        _teamStaffs = _teamStaffs.MergeBy(
            teams.Select(teamId => new TeamStaff(teamId, default, TeamMemberTypes.Member, UserId)),
            team => team.TeamId);
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
