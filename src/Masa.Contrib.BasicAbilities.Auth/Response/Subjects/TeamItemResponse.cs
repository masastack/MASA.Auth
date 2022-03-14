namespace Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

public class TeamItemResponse
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Describe { get; set; }

    public string CreatorUser { get; set; }

    public string CreatorUserAvatar { get; set; }

    public string ModifierUser { get; set; }

    public DateTime ModificationTime { get; set; }

    public TeamItemResponse(Guid teamId, string name, string avatar, string describe, string creatorUser, string creatorUserAvatar, string modifierUser, DateTime modificationTime)
    {
        TeamId = teamId;
        Name = name;
        Avatar = avatar;
        Describe = describe;
        CreatorUser = creatorUser;
        CreatorUserAvatar = creatorUserAvatar;
        ModifierUser = modifierUser;
        ModificationTime = modificationTime;
    }
}


