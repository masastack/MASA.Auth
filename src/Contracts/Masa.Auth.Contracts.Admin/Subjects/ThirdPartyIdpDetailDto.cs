namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDetailDto : ThirdPartyIdpDto
{   
    public ThirdPartyIdpDetailDto() : base()
    {

    }

    public ThirdPartyIdpDetailDto(Guid id, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime) : base(id, name, displayName, clientId, clientSecret, url, icon, authenticationType, creationTime, modificationTime)
    {
    }
}

