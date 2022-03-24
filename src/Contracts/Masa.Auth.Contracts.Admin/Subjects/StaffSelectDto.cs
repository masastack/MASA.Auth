namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffSelectDto
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public StaffSelectDto(Guid id, string jobNumber, string name, string avatar)
    {
        Id = id;
        JobNumber = jobNumber;
        Name = name;
        Avatar = avatar;
    }
}


