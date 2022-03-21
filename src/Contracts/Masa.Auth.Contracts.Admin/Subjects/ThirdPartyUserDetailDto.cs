namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyUserDetailDto
{
    public Guid Id { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public UserDetailDto User { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public static ThirdPartyUserDetailDto Default = new(default, default, true, "", UserDetailDto.Default, DateTime.Now, null, "","");

    public ThirdPartyUserDetailDto(Guid id, Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, UserDetailDto user, DateTime creationTime, DateTime? modificationTime, string creator, string modifier)
    {
        Id = id;
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
        Modifier = modifier;
    }
}


