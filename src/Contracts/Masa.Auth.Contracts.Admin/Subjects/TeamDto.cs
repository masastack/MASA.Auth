namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }

    public string Creator { get; set; }

    public string CreatorAvatar { get; set; }

    public string Modifier { get; set; }

    public DateTime? ModificationTime { get; set; }

    public TeamDto(Guid id, string name, string avatar, string description, string creator, string creatorAvatar, string modifier, DateTime? modificationTime)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Description = description;
        Creator = creator;
        CreatorAvatar = creatorAvatar;
        Modifier = modifier;
        ModificationTime = modificationTime;
    }
}


