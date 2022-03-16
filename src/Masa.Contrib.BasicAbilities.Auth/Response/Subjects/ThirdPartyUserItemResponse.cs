namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class ThirdPartyUserItemResponse
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public UserItemResponse User { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }

    public string Creator { get; set; }

    public ThirdPartyUserItemResponse(Guid thirdPartyUserId, Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, UserItemResponse user, DateTime creationTime, DateTime modificationTime, string creator)
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


