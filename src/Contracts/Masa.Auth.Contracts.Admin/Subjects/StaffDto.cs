namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid Id { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public StaffTypes StaffType { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public StaffDto(Guid id, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, string name, string avatar, string phoneNumber, string email)
    {
        Id = id;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        Name = name;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}


