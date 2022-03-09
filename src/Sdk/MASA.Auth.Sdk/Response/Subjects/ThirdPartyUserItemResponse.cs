namespace MASA.Auth.Sdk.Response.Subjects;

public class ThirdPartyUserItemResponse
{
    public Guid ThirdPartyUserId { get; set; }

    public Guid ThirdPartyPlatformId { get; private set; }

    public UserItemResponse User { get; set; }

    public UserStates UserState { get; private set; }

    public ThirdPartyUserItemResponse(Guid thirdPartyUserId, Guid thirdPartyPlatformId, UserItemResponse user, UserStates userState)
    {
        ThirdPartyUserId = thirdPartyUserId;
        ThirdPartyPlatformId = thirdPartyPlatformId;
        User = user;
        UserState = userState;
    }
}


