// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Options;

public class AliyunPhoneNumberLoginOptions : ConfigurationApiMasaConfigurationOptions
{
    [JsonIgnore]
    public override string AppId => "public-$Config";

    [JsonIgnore]
    public override string? ObjectName { get; } = "$public.AliyunPhoneNumberLogin";

    public string AccessKeyId { get; set; } = "";

    public string AccessKeySecret { get; set; } = "";

    public string Endpoint { get; set; } = "";
}
