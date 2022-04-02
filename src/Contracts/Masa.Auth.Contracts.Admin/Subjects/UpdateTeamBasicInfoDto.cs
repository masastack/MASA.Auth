namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateTeamBasicInfoDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public AvatarValueDto Avatar { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public TeamTypes Type { get; set; } = TeamTypes.Normal;
}
