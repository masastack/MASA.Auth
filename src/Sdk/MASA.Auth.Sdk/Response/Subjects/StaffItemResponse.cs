namespace Masa.Auth.Sdk.Response.Subjects;

public class StaffItemResponse
{
    public Guid StaffId { get; set; }

    public string JobNumber { get; private set; } 

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; private set; }

    public UserItemResponse User { get; set; }

    public StaffItemResponse(Guid staffId, string jobNumber, bool enabled, StaffTypes staffType, UserItemResponse user)
    {
        StaffId = staffId;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        User = user;
    }
}


