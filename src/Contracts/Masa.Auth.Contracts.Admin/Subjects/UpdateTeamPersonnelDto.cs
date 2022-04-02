namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateTeamPersonnelDto
{
    public Guid Id { get; set; }

    public List<Guid> Staffs { get; set; } = new();

    public List<Guid> Roles { get; set; } = new();

    public Dictionary<Guid, bool> Permissions { get; set; } = new();
}
