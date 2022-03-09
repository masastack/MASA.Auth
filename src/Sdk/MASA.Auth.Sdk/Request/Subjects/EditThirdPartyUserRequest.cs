namespace Masa.Auth.Sdk.Request.Subjects;

public class EditThirdPartyUserRequest
{
    public Guid ThirdPartyUserId { get; set; }

    public bool Enabled { get; set; }

    public EditUserRequest User { get; set; }

    public EditThirdPartyUserRequest(Guid thirdPartyUserId, bool enabled, EditUserRequest user)
    {
        ThirdPartyUserId = thirdPartyUserId;
        Enabled = enabled;
        User = user;
    }

    public static implicit operator EditThirdPartyUserRequest(ThirdPartyUserItemResponse thridPartyUser)
    {
        return new EditThirdPartyUserRequest(thridPartyUser.ThirdPartyUserId, thridPartyUser.Enabled, thridPartyUser.User);
    }
}
