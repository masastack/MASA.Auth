// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Builder;

public static class AuthenticationExternalAppBuilderExtensions
{
    public static IApplicationBuilder UseAuthorizationExternal(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationExternalMiddleware>();
    }
}
