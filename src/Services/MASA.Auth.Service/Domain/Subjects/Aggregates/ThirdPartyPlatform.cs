using Masa.Auth.Service.Infrastructure.Enums;

namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class ThirdPartyPlatform : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string ClientId { get; private set; }

    public string ClientSecret { get; private set; }

    public string Url { get; private set; }

    public string Icon { get; private set; }

    public PlatformTypes PlatformType { get; private set; }

    public VerifyTypes VerifyType { get; private set; }

    public IdentificationTypes IdentificationType { get; private set; }

    private ThirdPartyPlatform()
    {
        Name = "";
        DisplayName = "";
        ClientId = "";
        ClientSecret = "";
        Url = "";
        Icon = "";
    }

    public ThirdPartyPlatform(string name, string displayName, string clientId, string clientSecret, string url, string icon, PlatformTypes platformType, VerifyTypes verifyType, IdentificationTypes identificationType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        PlatformType = platformType;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }
}

