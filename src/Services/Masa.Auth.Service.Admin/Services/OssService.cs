// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Storage.ObjectStorage;

namespace Masa.Auth.Service.Admin.Services
{
    public class OssService : RestServiceBase
    {
        public OssService(IServiceCollection services) : base(services, "api/oss")
        {
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
        
        private async Task<List<GetDefaultImagesDto>> GetDefaultImagesAsync()
        {
            return new List<GetDefaultImagesDto>();
        }
    }
}
