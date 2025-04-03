// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class MasaStackConfigUtilsExtension
{
    public static JsonArray GetDefaultStackConfig(this IConfiguration configuration)
    {
        var value = configuration.GetValue<string>(MasaStackConfigConstant.MASA_STACK)!;
        if (string.IsNullOrEmpty(value))
        {
            return new();
        }
        return JsonSerializer.Deserialize<JsonArray>(value) ?? new();
    }

    public static string GetWebId(this JsonArray stackConfig, MasaStackProject project)
    {
        return stackConfig.GetId(project, MasaStackApp.WEB);
    }

    public static string GetId(this JsonArray stackConfig, MasaStackProject project, MasaStackApp app)
    {
        return stackConfig.FirstOrDefault(i => i?["id"]?.ToString() == project.Name)
            ?[app.Name]?["id"]?.ToString() ?? "";
    }

    public static string GetSsoDomain(this JsonArray stackConfig)
    {
        return stackConfig.GetDomain(MasaStackProject.Auth, MasaStackApp.SSO);
    }

    public static string GetDomain(this JsonArray stackConfig, MasaStackProject project, MasaStackApp app)
    {
        return stackConfig.FirstOrDefault(i => i?["id"]?.ToString() == project.Name)?[app.Name]?["domain"]?.ToString() ?? "";
    }
}
