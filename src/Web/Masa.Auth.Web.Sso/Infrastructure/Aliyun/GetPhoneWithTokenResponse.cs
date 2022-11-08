// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Aliyun;

public class GetPhoneWithTokenResponse
{
    public string? Code { get; set; }

    public string? Message { get; set; }

    public string? RequestId { get; set; }

    public PhoneResponse? Data { get; set; }
}

public class PhoneResponse
{
    public string? Mobile { get; set; }
}
