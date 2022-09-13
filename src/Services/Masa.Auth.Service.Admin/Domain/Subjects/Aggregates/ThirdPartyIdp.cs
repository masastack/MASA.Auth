// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class ThirdPartyIdp : IdentityProvider
{
    string _clientId = "";
    string _clientSecret = "";
    string _callbackPath = "";
    string _authorizationEndpoint = "";
    string _tokenEndpoint = "";
    string _userInformationEndpoint = "";
    AuthenticationTypes _authenticationType;
    string _jsonKeyMap = "";

    public string ClientId
    {
        get => _clientId;
        private set => _clientId = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(ClientId));
    }

    public string ClientSecret
    {
        get => _clientSecret;
        private set => _clientSecret = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(ClientSecret));
    }

    public string CallbackPath
    {
        get => _callbackPath;
        private set => _callbackPath = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(CallbackPath));
    }

    public string AuthorizationEndpoint
    {
        get => _authorizationEndpoint;
        private set => _authorizationEndpoint = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(AuthorizationEndpoint));
    }

    public string TokenEndpoint
    {
        get => _tokenEndpoint;
        private set => _tokenEndpoint = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(TokenEndpoint));
    }

    public string UserInformationEndpoint
    {
        get => _userInformationEndpoint;
        private set => _userInformationEndpoint = ArgumentExceptionExtensions.ThrowIfNullOrEmpty(value, nameof(UserInformationEndpoint));
    }

    public AuthenticationTypes AuthenticationType
    {
        get => _authenticationType;
        private set => _authenticationType = ArgumentExceptionExtensions.ThrowIfDefault(value, nameof(AuthenticationType));
    }

    public bool MapAll { get; private set; }

    [AllowNull]
    public string JsonKeyMap
    {
        get => _jsonKeyMap;
        set => _jsonKeyMap = value ?? "";
    }

    public ThirdPartyIdp(string name, string displayName, string icon, bool enabled, ThirdPartyIdpTypes thirdPartyIdpType, string clientId, string clientSecret, string callbackPath, string authorizationEndpoint, string tokenEndpoint, string userInformationEndpoint, AuthenticationTypes authenticationType, bool mapAll, string? jsonKeyMap)
    {
        Name = name;
        DisplayName = displayName;
        Icon = icon;
        Enabled = enabled;
        ThirdPartyIdpType = thirdPartyIdpType;
        ClientId = clientId;
        ClientSecret = clientSecret;
        CallbackPath = callbackPath;
        AuthorizationEndpoint = authorizationEndpoint;
        TokenEndpoint = tokenEndpoint;
        UserInformationEndpoint = userInformationEndpoint;
        AuthenticationType = authenticationType;
        MapAll = mapAll;
        JsonKeyMap = jsonKeyMap;
    }

    public void Update(string displayName, string icon, bool enabled, string clientId, string clientSecret, string callbackPath, string authorizationEndpoint, string tokenEndpoint, string userInformationEndpoint, bool mapAll, Dictionary<string,string> jsonKyMap)
    {
        DisplayName = displayName;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Icon = icon;
        Enabled = enabled;
        CallbackPath = callbackPath;
        AuthorizationEndpoint = authorizationEndpoint;
        TokenEndpoint = tokenEndpoint;
        UserInformationEndpoint = userInformationEndpoint;
        MapAll = mapAll;
        JsonKeyMap = JsonSerializer.Serialize(jsonKyMap);
    }
}

