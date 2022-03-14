namespace Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

public class ThirdPartyIdpItemResponse
{
    public Guid ThirdPartyIdpId { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? ModificationTime { get; set; }

    public static ThirdPartyIdpItemResponse Default => new ThirdPartyIdpItemResponse(Guid.Empty, "", "", "", "", "", "", default, DateTime.Now, null);

    public ThirdPartyIdpItemResponse(Guid thirdPartyIdpId, string name, string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType, DateTime creationTime, DateTime? modificationTime)
    {
        ThirdPartyIdpId = ThirdPartyIdpId;
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

