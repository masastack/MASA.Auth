// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class OssService : RestServiceBase
{
    public OssService() : base("api/oss")
    {
        MapGet(GetDefaultImages);
    }

    private async Task<SecurityTokenDto> GetSecurityTokenAsync([FromServices] IObjectStorageClient client, [FromServices] IMasaConfiguration masaConfiguration)
    {
        var region = "oss-cn-hangzhou";
        var response = client.GetSecurityToken();
        var stsToken = response.SessionToken;
        var accessId = response.AccessKeyId;
        var accessSecret = response.AccessKeySecret;
        var bucket = masaConfiguration.ConfigurationApi.GetPublic().GetSection(OssOptions.Key).Get<OssOptions>().Bucket;
        return await Task.FromResult(new SecurityTokenDto(region, accessId, accessSecret, stsToken, bucket));
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
