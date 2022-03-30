namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDetailDto
{
    public TeamBaseInfoDto TeamBaseInfo { get; set; } = new();

    public TeamPersonnelDto TeamAdmin { get; set; } = new();

    public TeamPersonnelDto TeamMember { get; set; } = new();
}

public class TeamBaseInfoDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public AvatarValueDto Avatar { get; set; } = new AvatarValueDto();

    public string Description { get; set; } = String.Empty;

    public TeamTypes TeamType { get; set; }
}

public class TeamPersonnelDto
{
    public List<Guid> Staffs { get; set; } = new();

    public List<Guid> Permissions { get; set; } = new();

    public List<Guid> Roles { get; set; } = new();
}
