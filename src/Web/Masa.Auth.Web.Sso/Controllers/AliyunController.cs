// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AlibabaCloud.SDK.Dypnsapi20170525.Models.GetAuthTokenResponseBody;

namespace Masa.Auth.Web.Sso.Controllers;

[AllowAnonymous]
public class AliyunController : Controller
{
    readonly LocalLoginByPhoneNumberAgent _localLoginByPhoneNumberAgent;

    public AliyunController(LocalLoginByPhoneNumberAgent localLoginByPhoneNumberAgent)
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
