namespace Masa.Auth.Sdk.Response.Subjects;

public class PlatformItemResponse
{
    public Guid PlatformId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public PlatformItemResponse(Guid platformId, string name, string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType, DateTime creationTime, DateTime? modificationTime)
    {
        PlatformId = platformId;
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
        CreationTime = creationTime;
        ModificationTime = modificationTime;
    }
}

public enum VerifyType
{
    OAuth
}

