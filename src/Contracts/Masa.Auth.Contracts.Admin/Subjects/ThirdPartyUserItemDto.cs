namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyUserDto
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public UserDto User { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }

    public string Creator { get; set; }

    public ThirdPartyUserDto(Guid thirdPartyUserId, Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, UserDto user, DateTime creationTime, DateTime modificationTime, string creator)
    {
        ThirdPartyUserId = thirdPartyUserId;
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
    }
}


