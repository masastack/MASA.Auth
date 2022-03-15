﻿using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class AddThirdPartyIdpRequest
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public VerifyType VerifyType { get; set; }

    public AddThirdPartyIdpRequest(string name, string displayName, string clientId, string clientSecret, string url, string icon, VerifyType verifyType)
    {
        Name = name;
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Url = url;
        Icon = icon;
        VerifyType = verifyType;
    }

    public static implicit operator AddThirdPartyIdpRequest(ThirdPartyIdpItemResponse platform)
    {
        return new AddThirdPartyIdpRequest(platform.Name, platform.DisplayName, platform.ClientId, platform.ClientSecret, platform.Url, platform.Icon, platform.VerifyType);
    }
}
