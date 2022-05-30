// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Oss;

public class GetSecurityTokenDto
{
    public string Region { get; set; }

    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string StsToken { get; set; }

    public string Bucket { get; set; }

    public GetSecurityTokenDto(string region, string accessKeyId, string accessKeySecret, string stsToken, string bucket)
    {
        Region = region;
        AccessKeyId = accessKeyId;
        AccessKeySecret = accessKeySecret;
        StsToken = stsToken;
        Bucket = bucket;
    }
}

