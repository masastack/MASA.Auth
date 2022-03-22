namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public TeamSelectDto(Guid id, string name, string avatar)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
    }
}
