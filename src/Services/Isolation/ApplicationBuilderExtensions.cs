// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;

namespace Masa.Contrib.StackSdks.Isolation;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseStackIsolation(this WebApplication app)
    {
        app.UseIsolation();
        app.UseMiddleware<EnvironmentMiddleware>();
        return app;
    }
}
