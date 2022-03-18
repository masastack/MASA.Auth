namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDetailDto : ThirdPartyIdpIDto
{
    public static ThirdPartyIdpDetailDto Default => new ThirdPartyIdpDetailDto(Guid.Empty, "", "", "", "", "", "", default, DateTime.Now, null);

    public ThirdPartyIdpDetailDto(Guid thirdPartyIdpId, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime) : base(thirdPartyIdpId, name, displayName, clientId, clientSecret, url, icon, authenticationType, creationTime, modificationTime)
    {
    }
}

