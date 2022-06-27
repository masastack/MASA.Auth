// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class OssService : RestServiceBase
{
    public OssService(IServiceCollection services) : base(services, "api/oss")
    {
        MapGet(GetDefaultImages);
    }

    private async Task<GetSecurityTokenDto> GetSecurityTokenAsync([FromServices] IClient client, [FromServices] DaprClient daprClient)
    {
        var region = "oss-cn-hangzhou";
        var response = client.GetSecurityToken();
        var stsToken = response.SessionToken;
        var accessId = response.AccessKeyId;
        var accessSecret = response.AccessKeySecret;
        var bucket = daprClient.GetSecretAsync("localsecretstore", "bucket").Result.First().Value;
        return await Task.FromResult(new GetSecurityTokenDto(region, accessId, accessSecret, stsToken, bucket));
    }

    private List<GetDefaultImagesDto> GetDefaultImages()
    {
        return new List<GetDefaultImagesDto>()
        {
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.chen.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.gu.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.ma.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.wu.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.xie.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.yan.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.zhen.svg"),
            new GetDefaultImagesDto(GenderTypes.Male,"https://cdn.masastack.com/stack/images/avatar/mr.zhu.svg"),
            new GetDefaultImagesDto(GenderTypes.Female,"https://cdn.masastack.com/stack/images/avatar/ms.qu.svg"),
            new GetDefaultImagesDto(GenderTypes.Female,"https://cdn.masastack.com/stack/images/avatar/ms.wu.svg"),
        };
    }
}
