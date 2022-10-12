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

    public GetAuthTokenResponseBodyTokenInfo? GetAuthTokenResponseBodyTokenInfo(GetAuthTokenRequest? request)
    {
        var client = CreateClient();
        request ??= new GetAuthTokenRequest
        {
            Url = "https://0.0.0.0",
            Origin = "https://0.0.0.0",
        };
        var describePhoneNumberResaleResp = client.GetAuthToken(request);
        string code = describePhoneNumberResaleResp.Body.Code;
        if (code == "OK")
        {
            return describePhoneNumberResaleResp.Body.TokenInfo;
        }
        return null;
    }

    public bool VerifyPhoneWithToken(string phoneNumber, string spToken, out string errorMsg)
    {
        var client = CreateClient();
        var verifyPhoneWithTokenRequest = new VerifyPhoneWithTokenRequest
        {
            PhoneNumber = phoneNumber,
            SpToken = spToken,
        };
        var runtime = new RuntimeOptions();
        var verifyPhoneWithTokenResponse = client.VerifyPhoneWithTokenWithOptions(verifyPhoneWithTokenRequest, runtime);        
        if (verifyPhoneWithTokenResponse.Body.Code == "OK")
        {
            var verifyResult = verifyPhoneWithTokenResponse.Body.GateVerify.VerifyResult;
            switch (verifyResult)
            {
                case "PASS":
                    errorMsg = "";
                    return true;
                case "REJECT":
                    errorMsg = "号码验证不一致，请通过短信方式登录";
                    return false;
                case "UNKNOWN":
                    errorMsg = "无法判断号码，请通过短信方式登录";
                    return false;
                default:
                    errorMsg = "未知异常";
                    return false;
            };
        }
        errorMsg = "运营商接口异常";
        return false;  
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
