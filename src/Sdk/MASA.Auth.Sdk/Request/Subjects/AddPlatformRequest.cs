using Masa.Auth.Sdk.Response.Subjects;

namespace Masa.Auth.Sdk.Request.Subjects;

public class AddPlatformRequest
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public AddPlatformRequest(string name, string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
    }

    public static implicit operator AddPlatformRequest(PlatformItemResponse request)
    {
        return new AddPlatformRequest(request.Name, request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.VerifyType);
    }
}
