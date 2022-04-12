namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyIdp : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string ClientId { get; private set; }

    public string ClientSecret { get; private set; }

    public string Url { get; private set; }

    public string Icon { get; private set; }

    public AuthenticationTypes VerifyType { get; private set; }

    public IdentificationTypes IdentificationType { get; private set; }

    public ThirdPartyIdp(string name, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes verifyType, IdentificationTypes identificationType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
        IdentificationType = identificationType;
    }
}

