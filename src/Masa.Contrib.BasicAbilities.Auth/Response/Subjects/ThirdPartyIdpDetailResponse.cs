namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class ThirdPartyIdpDetailResponse : ThirdPartyIdpItemResponse
{
    public static ThirdPartyIdpDetailResponse Default => new ThirdPartyIdpDetailResponse(Guid.Empty, "", "", "", "", "", "", default, DateTime.Now, null);

    public ThirdPartyIdpDetailResponse(Guid thirdPartyIdpId, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime) : base(thirdPartyIdpId, name, displayName, clientId, clientSecret, url, icon, authenticationType, creationTime, modificationTime)
    {
    }
}

