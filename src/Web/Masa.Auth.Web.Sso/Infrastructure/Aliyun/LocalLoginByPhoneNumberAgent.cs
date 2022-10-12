// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using AliyunClient = AlibabaCloud.SDK.Dypnsapi20170525.Client;
using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;

namespace Masa.Auth.Web.Sso.Infrastructure.Aliyun;

public class LocalLoginByPhoneNumberAgent : IScopedDependency
{
    AliyunPhoneNumberLoginOptions _options;

    public LocalLoginByPhoneNumberAgent(IOptions<AliyunPhoneNumberLoginOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GetAuthTokenResponseBodyTokenInfo?> GetAuthTokenAsync(GetAuthTokenRequest? request = null)
    {
        var client = CreateClient();
        request ??= new GetAuthTokenRequest
        {
            Url = "https://0.0.0.0",
            Origin = "https://0.0.0.0",
        };
        var describePhoneNumberResaleResp = await client.GetAuthTokenAsync(request);
        string code = describePhoneNumberResaleResp.Body.Code;
        if (code == "OK")
        {
            return describePhoneNumberResaleResp.Body.TokenInfo;
        }
        return null;
    }

    public async Task<(bool success,string errorMsg)> VerifyPhoneWithTokenAsync(string phoneNumber, string spToken)
    {
        var client = CreateClient();
        var verifyPhoneWithTokenRequest = new VerifyPhoneWithTokenRequest
        {
            PhoneNumber = phoneNumber,
            SpToken = spToken,
        };
        var runtime = new RuntimeOptions();
        try
        {
            var verifyPhoneWithTokenResponse = await client.VerifyPhoneWithTokenWithOptionsAsync(verifyPhoneWithTokenRequest, runtime);
            if (verifyPhoneWithTokenResponse.Body.Code == "OK")
            {
                var verifyResult = verifyPhoneWithTokenResponse.Body.GateVerify.VerifyResult;
                switch (verifyResult)
                {
                    case "PASS":
                        return (true, "");
                    case "REJECT":
                        return (false, "号码验证不一致，请通过短信方式登录");
                    case "UNKNOWN":
                        return (false, "无法判断号码，请通过短信方式登录");
                    default:
                        return (false, "未知异常");
                };
            }
        }
        catch
        {
        }
        return (false, "运营商接口异常");
    }

    private AliyunClient CreateClient()
    {
        var config = new AliyunConfig
        {
            AccessKeyId = _options.AccessKeyId,
            AccessKeySecret = _options.AccessKeySecret,
            Endpoint = _options.Endpoint,
        };
        return new AliyunClient(config);
    }
}
