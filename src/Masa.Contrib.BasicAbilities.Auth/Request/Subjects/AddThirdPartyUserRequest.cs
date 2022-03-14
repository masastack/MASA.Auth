using Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class AddThirdPartyUserRequest
{
    public Guid ThirdPartyPlatformId { get; set; }

    public bool Enabled { get; set; }

    public AddUserRequest User { get; set; }

    public AddThirdPartyUserRequest(Guid thirdPartyPlatformId, bool enabled, AddUserRequest user)
    {
        ThirdPartyPlatformId = thirdPartyPlatformId;
        Enabled = enabled;
        User = user;
    }

    public static implicit operator AddThirdPartyUserRequest(ThirdPartyUserItemResponse thridPartyUser)
    {
        return new AddThirdPartyUserRequest(thridPartyUser.ThirdPartyPlatformId, thridPartyUser.Enabled, thridPartyUser.User);
    }
}
