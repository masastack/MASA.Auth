namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class AddThirdPartyUserRequest
{
    public Guid ThirdPartyIdpId { get; set; }

    public bool Enabled { get; set; }

    public string ThridPartyIdentity { get; set; }

    public AddUserRequest User { get; set; }

    public AddThirdPartyUserRequest(Guid thirdPartyIdpId, bool enabled, string thridPartyIdentity, AddUserRequest user)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Enabled = enabled;
        ThridPartyIdentity = thridPartyIdentity;
        User = user;
    }

    public static implicit operator AddThirdPartyUserRequest(ThirdPartyUserDetailResponse thridPartyUser)
    {
        return new AddThirdPartyUserRequest(thridPartyUser.ThirdPartyIdpId, thridPartyUser.Enabled, thridPartyUser.ThridPartyIdentity, thridPartyUser.User);
    }
}
