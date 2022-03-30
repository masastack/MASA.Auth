namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid Id { get; set; }

    public string Department { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public string JobNumber { get; set; }

    public bool Enabled { get; private set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public StaffDto(Guid id, string name, string displayName, string phoneNumber, string email, string jobNumber, string avatar)
    {
        Id = id;
        JobNumber = jobNumber;
        Name = name;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
        DisplayName = displayName;
    }

    public StaffDto(Guid id, string department, string position, string jobNumber, bool enabled, string name, string displayName, string avatar, string phoneNumber, string email)
    {
        Id = id;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        Name = name;
        Avatar = avatar;
        DisplayName = displayName;
    }
}


