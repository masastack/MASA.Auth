namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDetailDto
{
    public Guid Id { get; set; }

    public TeamBasicInfoDto TeamBasicInfo { get; set; } = new();

    public TeamPersonnelDto TeamAdmin { get; set; } = new();

    public TeamPersonnelDto TeamMember { get; set; } = new();
}

public class TeamBasicInfoDto
{

    public string Name { get; set; } = string.Empty;

    public AvatarValueDto Avatar { get; set; } = new AvatarValueDto();

    public string Description { get; set; } = string.Empty;

    public int Type { get; set; }

}

public class TeamPersonnelDto
{
    public List<Guid> Staffs { get; set; } = new();

    public Dictionary<Guid, bool> Permissions { get; set; } = new();

    public List<Guid> Roles { get; set; } = new();
}
