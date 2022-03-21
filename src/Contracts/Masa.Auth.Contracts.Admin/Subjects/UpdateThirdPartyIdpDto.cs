namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public AuthenticationTypes AuthenticationType { get; set; }

    public UpdateThirdPartyIdpDto(Guid id, string displayName, string clientId, string clientSecret, string url, string icon, AuthenticationTypes authenticationType)
    {
        Id = id;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        AuthenticationType = authenticationType;
    }

    public static implicit operator UpdateThirdPartyIdpDto(ThirdPartyIdpDetailDto request)
    {
        return new UpdateThirdPartyIdpDto(request.Id, request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.AuthenticationType);
    }
}
