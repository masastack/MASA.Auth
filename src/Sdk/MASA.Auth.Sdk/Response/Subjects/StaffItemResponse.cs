namespace Masa.Auth.Sdk.Response.Subjects;

public class StaffItemResponse
{
    public Guid StaffId { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public UserItemResponse User { get; set; }

    public StaffItemResponse(Guid staffId, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, UserItemResponse user)
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


