namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid StaffId { get; set; }

    public string Department { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public StaffDto(Guid staffId, string name, string phoneNumber, string email, string jobNumber, string avatar)
    {
        StaffId = staffId;
        JobNumber = jobNumber;
        Name = name;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public StaffDto(Guid staffId, string department, string position, string jobNumber, bool enabled, string name, string avatar)
    {
        StaffId = staffId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        Name = name;
        Avatar = avatar;
    }
}


