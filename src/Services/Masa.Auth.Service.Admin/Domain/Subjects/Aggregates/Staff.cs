// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Staff : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    private User? _user;
    private Position? _position;
    private List<DepartmentStaff> _departmentStaffs = new();
    private List<TeamStaff> _teamStaffs = new();
    private Guid? _positionId;
    private User? _createUser;
    private User? _modifyUser;

    public bool IsDeleted { get; private set; }

    public virtual User User => _user ?? throw new UserFriendlyException("Failed to get user data");

    public virtual Position? Position => _position;

    public virtual IReadOnlyList<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public virtual IReadOnlyList<TeamStaff> TeamStaffs => _teamStaffs;

    public User? CreateUser => _createUser;

    public User? ModifyUser => _modifyUser;

    public Guid UserId { get; private set; }

    public string JobNumber { get; private set; } = "";

    /// <summary>
    /// redundance user name
    /// </summary>
    public string Name { get; private set; } = "";

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

    public Staff(Guid userId, string jobNumber, string name, Guid? positionId, StaffTypes staffType, bool enabled)
    {
        UserId = userId;
        JobNumber = jobNumber;
        Name = name;
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public static implicit operator StaffDetailDto(Staff staff)
    {
        var teams = staff.TeamStaffs.Select(t => t.TeamId).ToList();
        var departmentStaff = staff.DepartmentStaffs.FirstOrDefault();
        return new(staff.Id, departmentStaff?.DepartmentId ?? default, departmentStaff?.Department?.Name ?? "", staff.PositionId ?? default, staff.Position?.Name ?? "", staff.JobNumber, staff.Enabled && staff.User.Enabled, staff.StaffType, teams, new(), staff.CreateUser?.Name ?? "", staff.ModifyUser?.Name ?? "", staff.CreationTime, staff.ModificationTime);
    }

    public void Update(string name, Guid? positionId, StaffTypes staffType, bool enabled)
    {
        Name = name;
        PositionId = positionId;
        StaffType = staffType;
        Enabled = enabled;
    }

    public void AddDepartmentStaff(Guid departmentId)
    {
        _departmentStaffs.Clear();
        if (departmentId != default) _departmentStaffs.Add(new DepartmentStaff(departmentId, Guid.Empty));
    }

    public void AddTeamStaff(List<Guid> teams)
    {
        _teamStaffs.Clear();
        foreach (var teamId in teams)
        {
            _teamStaffs.Add(new TeamStaff(teamId, default, TeamMemberTypes.Member));
        }
    }
}
