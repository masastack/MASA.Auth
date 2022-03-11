namespace Masa.Auth.Sdk.Response.Subjects;

public class StaffItemResponse
{
    public Guid StaffId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; }

    public List<Guid> TeamIds { get; set; }

    public string JobNumber { get; set; } 

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public UserItemResponse User { get; set; }

    public StaffItemResponse(Guid staffId, Guid departmentId, Guid positionId,string position, List<Guid> teamIds, string jobNumber, bool enabled, StaffTypes staffType, UserItemResponse user)
    {
        StaffId = staffId;
        DepartmentId = departmentId;
        JobNumber = jobNumber;
        PositionId = positionId;
        Position = position;
        TeamIds = teamIds;
        Enabled = enabled;
        StaffType = staffType;
        User = user;
    }
}


