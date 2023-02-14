// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Controllers;

[AllowAnonymous]
public class ThirdPartyLoginController : Controller
{
    readonly LocalLoginByPhoneNumberAgent _localLoginByPhoneNumberAgent;

    public ThirdPartyLoginController(LocalLoginByPhoneNumberAgent localLoginByPhoneNumberAgent)
    {
        _localLoginByPhoneNumberAgent = localLoginByPhoneNumberAgent;
    }

    [HttpGet("aliyun/authToken")]
    public Task<GetAuthTokenResponseBodyTokenInfo?> GetAuthTokenAsync()
    {
        return _localLoginByPhoneNumberAgent.GetAuthTokenAsync();
    }

    [HttpGet("aliyun/phoneWithToken")]
    public GetPhoneWithTokenResponse? GetPhoneWithToken([FromQuery] string token)
    {
        return _localLoginByPhoneNumberAgent.GetPhoneWithToken(token);
    }
}
