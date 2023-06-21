// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace Masa.Contrib.StackSdks.Isolation;

internal class IsolationConfigurationApi : IConfigurationApi
{
    readonly IConfiguration _configuration;
    readonly IHttpContextAccessor _httpContextAccessor;

    public IsolationConfigurationApi(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public IConfiguration Get(string appId)
    {
        var multiEnvironmentContext = _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IMultiEnvironmentContext>();
        return _configuration.GetSection(SectionTypes.ConfigurationApi.ToString()).GetSection(multiEnvironmentContext.CurrentEnvironment).GetSection(appId);
    }
}

