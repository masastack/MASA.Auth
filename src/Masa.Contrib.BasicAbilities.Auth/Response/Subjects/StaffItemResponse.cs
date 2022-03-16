namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class StaffItemResponse
{
    public Guid StaffId { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public MemberTypes StaffType { get; set; }

    public UserItemResponse User { get; set; }

    public StaffItemResponse(Guid staffId, string department, string position, string jobNumber, bool enabled, MemberTypes staffType, UserItemResponse user)
    {
        StaffId = staffId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        User = user;
    }
}


