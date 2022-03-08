using Masa.Auth.Sdk.Response.Subjects;

namespace Masa.Auth.Sdk.Request.Subjects;

public class EditPlatformRequest
{
    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public EditPlatformRequest(string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType)
    {
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
    }

    public static implicit operator EditPlatformRequest(PlatformItemResponse request)
    {
        return new EditPlatformRequest(request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.VerifyType);
    }
}
