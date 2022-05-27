// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Client = Masa.Contrib.Storage.ObjectStorage.Aliyun.Client;

namespace Masa.Auth.Service.Admin.Services
{
    public class OssService : RestServiceBase
    {
        public OssService(IServiceCollection services) : base(services, "api/oss")
        {
        }

        private async Task<GetAccessTokenDto> GetAccessTokenAsync([FromServices] DaprClient daprClient, [FromServices] Client client)
        {
            var accessId = await daprClient.GetSecretAsync("localsecretstore", "access_id");
            var accessSecret = await daprClient.GetSecretAsync("localsecretstore", "access_secret");
            var bucket = await daprClient.GetSecretAsync("localsecretstore", "bucket");
            client.GetSecurityToken();

            return new();
        }      
    }
}
