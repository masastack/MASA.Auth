namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class ThirdPartyIdpItemResponse
{
    public Guid ThirdPartyIdpId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public AuthenticationTypes AuthenticationType { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public ThirdPartyIdpItemResponse(Guid thirdPartyIdpId, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        AuthenticationType = authenticationType;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
    }
}

