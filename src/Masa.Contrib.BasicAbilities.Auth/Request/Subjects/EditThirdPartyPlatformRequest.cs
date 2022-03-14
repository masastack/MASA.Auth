using Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class EditThirdPartyPlatformRequest
{
    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public EditThirdPartyPlatformRequest(string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType)
    {
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
    }

    public static implicit operator EditThirdPartyPlatformRequest(ThirdPartyPlatformItemResponse request)
    {
        return new EditThirdPartyPlatformRequest(request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.VerifyType);
    }
}
