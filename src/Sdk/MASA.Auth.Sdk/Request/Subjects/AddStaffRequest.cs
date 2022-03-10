namespace Masa.Auth.Sdk.Request.Subjects;

public class AddStaffRequest
{
    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public AddUserRequest User { get; set; }

    public AddStaffRequest(string jobNumber, StaffTypes staffType, bool enabled, AddUserRequest user)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        User = user;
    }

    public static implicit operator AddStaffRequest(StaffItemResponse staff)
    {
        return new AddStaffRequest(staff.JobNumber, staff.StaffType, staff.Enabled, staff.User);
    }
}
