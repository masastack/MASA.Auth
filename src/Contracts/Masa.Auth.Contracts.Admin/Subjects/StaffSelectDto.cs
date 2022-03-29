namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffSelectDto
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;
}
