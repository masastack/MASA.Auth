namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Department { get; set; } = "";

    public string Position { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public bool Enabled { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public StaffDto()
    {

    }

    public StaffDto(Guid id, Guid userId, string department, string position, string jobNumber, bool enabled, string name, string displayName, string avatar, string phoneNumber, string email)
    {
        Id = id;
        UserId = userId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}


