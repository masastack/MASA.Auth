// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
}
