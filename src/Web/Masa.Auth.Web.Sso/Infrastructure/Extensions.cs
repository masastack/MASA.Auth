// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public static class Extensions
{
    public static async Task MigrateDbContextAsync<TContext>(this WebApplicationBuilder builder) where TContext : DbContext
    {
        var services = builder.Services.BuildServiceProvider();
        var context = services.GetRequiredService<TContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}
