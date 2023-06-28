﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class TestService : ServiceBase
{
    public TestService() : base("api/test")
    {
        MapGet(GetConf, "conf");
    }

    public string GetConf([FromServices] IMasaConfiguration masaConfiguration, [FromServices] IConfiguration configuration, string Environment = "")
    {
        var d = configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection("Test").GetSection("public-$Config");
        var ddd = d.GetSection("$public.OSS").Get(typeof(OssOptions));
        var suffix = masaConfiguration.ConfigurationApi.GetPublic()
            .GetValue("$public.DefaultConfig:SUFFIX_IDENTITY", "empty");
        return suffix;
    }
}
