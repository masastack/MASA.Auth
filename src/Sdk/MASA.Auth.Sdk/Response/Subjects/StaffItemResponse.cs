namespace Masa.Auth.Sdk.Response.Subjects;

public class StaffItemResponse
{
    public Guid StaffId { get; set; }

    public UserItemResponse User { get; set; }

    public string JobNumber { get; private set; } 

    public StaffStates StaffState { get; private set; }

    public StaffTypes StaffType { get; private set; }

    public StaffItemResponse(Guid staffId, UserItemResponse user, string jobNumber, StaffStates staffState, StaffTypes staffType)
    {
        StaffId = staffId;
        User = user;
        JobNumber = jobNumber;
        StaffState = staffState;
        StaffType = staffType;
    }
}


