namespace Masa.Auth.Sdk.Response.Subjects;

public class ThirdPartyUserItemResponse
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyPlatformId { get; set; }

    public bool Enabled { get; set; }

    public UserItemResponse User { get; set; }

    public ThirdPartyUserItemResponse(Guid thirdPartyUserId, Guid thirdPartyPlatformId, bool enabled, UserItemResponse user)
    {
        ThirdPartyUserId = thirdPartyUserId;
        ThirdPartyPlatformId = thirdPartyPlatformId;
        User = user;
        Enabled = enabled;
    }
}


