namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public AuthenticationTypes AuthenticationType { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public ThirdPartyIdpDto()
    {
        Name = "";
        DisplayName = "";
        ClientId = "";
        ClientSecret = "";
        Url = "";
        Icon = "";
    }

    public ThirdPartyIdpDto(Guid id, string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType, DateTime creationTime, DateTime? modificationTime)
    {
        Id = id;
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

