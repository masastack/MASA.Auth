namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDto
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Describe { get; set; }

    public string Creator { get; set; }

    public string CreatorAvatar { get; set; }

    public string Modifier { get; set; }

    public DateTime? ModificationTime { get; set; }

    public TeamDto(Guid teamId, string name, string avatar, string describe, string creator, string creatorAvatar, string modifier, DateTime? modificationTime)
    {
        TeamId = teamId;
        Name = name;
        Avatar = avatar;
        Describe = describe;
        Creator = creator;
        CreatorAvatar = creatorAvatar;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}


