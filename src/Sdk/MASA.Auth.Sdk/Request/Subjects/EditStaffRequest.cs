namespace Masa.Auth.Sdk.Request.Subjects;

public class EditStaffRequest
{
    public Guid StaffId { get; set; }

    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public EditUserRequest User { get; set; }

    public EditStaffRequest(Guid staffId, string jobNumber, StaffTypes staffType, bool enabled, EditUserRequest user)
    {
        StaffId = staffId;
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        User = user;
    }

    public static implicit operator EditStaffRequest(StaffItemResponse staff)
    {
        return new EditStaffRequest(staff.StaffId, staff.JobNumber, staff.StaffType, staff.Enabled, staff.User);
    }
}
