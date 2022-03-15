﻿using Masa.Auth.ApiGateways.Caller.Enums;

namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class StaffDetailResponse
{
    public Guid StaffId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public List<Guid> TeamIds { get; set; }

    public UserItemResponse User { get; set; }

    public static StaffDetailResponse Default => new StaffDetailResponse(Guid.Empty, Guid.Empty, Guid.Empty, "", "", true, default, new(), UserItemResponse.Default);

    public StaffDetailResponse(Guid staffId, Guid departmentId, Guid positionId, string position, string jobNumber, bool enabled, StaffTypes staffType, List<Guid> teamIds, UserItemResponse user)
    {
        StaffId = staffId;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        TeamIds = teamIds;
        User = user;
    }
}


