using Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class AddThirdPartyUserRequest
{
    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public AddUserRequest User { get; set; }

    public AddThirdPartyUserRequest(Guid thirdPartyIdpId, bool enabled, AddUserRequest user)
    {
        ThirdPartyIdpId = ThirdPartyIdpId;
        Enabled = enabled;
        User = user;
    }

    public static implicit operator AddThirdPartyUserRequest(ThirdPartyUserItemResponse thridPartyUser)
    {
        return new AddThirdPartyUserRequest(thridPartyUser.ThirdPartyIdpId, thridPartyUser.Enabled, thridPartyUser.User);
    }
}
