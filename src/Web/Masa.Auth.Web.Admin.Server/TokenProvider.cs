// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Server;

public class TokenProvider : ITokenProvider
{
    readonly BlazorServerTokenData _blazorServerTokenData;

    public TokenProvider(IUserContext userContext, BlazorServerTokenCache blazorServerTokenCache)
    {
        _blazorServerTokenData = blazorServerTokenCache.Get(userContext.UserId ?? "") ?? new();
    }

    public string? AccessToken => _blazorServerTokenData.AccessToken;

    public string? RefreshToken => _blazorServerTokenData.RefreshToken;

    public string? IdToken => _blazorServerTokenData.RefreshToken;
}
