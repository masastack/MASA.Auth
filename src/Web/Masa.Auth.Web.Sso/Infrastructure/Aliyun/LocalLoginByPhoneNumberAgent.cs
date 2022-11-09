// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Http;
using AliyunClient = AlibabaCloud.SDK.Dypnsapi20170525.Client;
using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;

namespace Masa.Auth.Web.Sso.Infrastructure.Aliyun;

public class LocalLoginByPhoneNumberAgent : IScopedDependency
{
    readonly ILogger _logger;
    readonly AliyunPhoneNumberLoginOptions _options;

    public LocalLoginByPhoneNumberAgent(IOptions<AliyunPhoneNumberLoginOptions> options, ILoggerFactory loggerFactory)
    {
        _options = options.Value;
        _logger = loggerFactory.CreateLogger<LocalLoginByPhoneNumberAgent>();
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

    public GetPhoneWithTokenResponse? GetPhoneWithToken(string spToken)
    {
        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", _options.AccessKeyId, _options.AccessKeySecret);
        DefaultAcsClient client = new DefaultAcsClient(profile);
        try
        {
            var request = new CommonRequest();
            request.Method = MethodType.POST;
            request.Domain = _options.Endpoint;
            request.Version = "2017-05-25";
            request.Action = "GetPhoneWithToken";
            request.AddQueryParameters("SpToken", spToken);
            CommonResponse response = client.GetCommonResponse(request);
            var resultStr = Encoding.Default.GetString(response.HttpResponse.Content);
            _logger.LogInformation(resultStr);
            return JsonSerializer.Deserialize<GetPhoneWithTokenResponse>(resultStr);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return default;
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
