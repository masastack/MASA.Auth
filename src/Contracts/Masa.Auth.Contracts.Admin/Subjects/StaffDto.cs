namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid StaffId { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes MemberType { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public StaffDto(Guid staffId, string department, string position, string jobNumber, bool enabled, StaffTypes memberType, string name, string avatar, string phoneNumber, string email)
    {
        StaffId = staffId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        MemberType = memberType;
        Name = name;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}


