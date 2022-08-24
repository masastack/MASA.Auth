// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : FullAggregateRoot<Guid, Guid>
{
    private User? _user;
    private Position? _position;
    private List<DepartmentStaff> _departmentStaffs = new();
    private List<TeamStaff> _teamStaffs = new();
    private User? _createUser;
    private User? _modifyUser;

    private Guid _userId;
    private string _name = "";
    private string _displayName = "";
    private string _avatar = "";
    private string _idCard = "";
    private string _companyName = "";
    private Guid? _positionId;
    private string _phoneNumber = "";
    private string _email = "";
    private GenderTypes _gender;
    private AddressValue _address = new();
    private string _jobNumber = "";
    private StaffTypes _staffType;

    public Guid UserId
    {
        get => _userId;
        private set => _userId = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(UserId));
    }

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

    [AllowNull]
    public string CompanyName
    {
        get => _companyName;
        private set => _companyName = value ?? "";
    }

    public GenderTypes Gender
    {
        get => _gender;
        private set
        {
            _gender = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(Gender));
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

    public string JobNumber
    {
        get => _jobNumber;
        private set => _jobNumber = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(JobNumber));
    }

    public Guid? PositionId
    {
        get => _positionId;
        set
        {
            _positionId = value == Guid.Empty ? null : value;
        }
    }

    public StaffTypes StaffType
    {
        get => _staffType;
        private set
        {
            _staffType = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(StaffType));
        }
    }

    public bool Enabled { get; private set; }

    public User User => _user ?? throw new UserFriendlyException("Failed to get user data");

    public Position? Position => _position;

    public IReadOnlyList<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public IReadOnlyList<TeamStaff> TeamStaffs => _teamStaffs;

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

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
        AddressValue? address)
    {
        Name = name;
        IdCard = idCard;
        CompanyName = companyName;
        Address = address;
        PositionId = positionId;
        Enabled = enabled;
        UserId = userId;
        JobNumber = jobNumber;
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

    public void Update(Guid? positionId, StaffTypes staffType, bool enabled, string? name, string displayName, string avatar, string? idCard, string? companyName, string? phoneNumber, string? email, AddressValue? address, GenderTypes gender)
    {
        Name = name;
        PositionId = positionId;
        Enabled = enabled;
        Name = name;
        IdCard = idCard;
        Avatar = avatar;
        CompanyName = companyName;
        Enabled = enabled;
        Address = address;
        DisplayName = displayName;
        StaffType = staffType;
        Gender = gender;
        VerifyPhonNumberEmail(phoneNumber, email);
    }

    public void UpdateForLdap(string? name, string displayName, string phoneNumber, string? email)
    {
        Name = name;
        DisplayName = displayName;
        PhoneNumber = phoneNumber;
        Email = email;
        VerifyPhonNumberEmail(phoneNumber, email);
    }

    public void UpdateBasicInfo(string? name, string displayName, GenderTypes gender, Guid? positionId, StaffTypes staffType)
    {
        Name = name;
        PositionId = positionId;
        Name = name;
        DisplayName = displayName;
        StaffType = staffType;
        Gender = gender;
    }

    public void UpdateAvatar(string avatar)
    {
        Avatar = avatar;
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
