namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class ThirdPartyUserItemResponse
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public UserItemResponse User { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }

    public Guid Creator { get; set; }

    public static ThirdPartyUserItemResponse Default = new(default, default, true, UserItemResponse.Default, default, default, default);

    public ThirdPartyUserItemResponse(Guid thirdPartyUserId, Guid thirdPartyIdpId, bool enabled, UserItemResponse user, DateTime creationTime, DateTime modificationTime, Guid creator)
    {
        ThirdPartyUserId = thirdPartyUserId;
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        User = user;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
        Creator = creator;
    }
}


