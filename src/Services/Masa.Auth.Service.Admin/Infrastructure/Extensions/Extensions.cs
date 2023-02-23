// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class Extensions
{
    public static bool WildCardContains(this IEnumerable<string> data, string code)
    {
        return data.Any(item => Regex.IsMatch(code.ToLower(),
            Regex.Escape(item.ToLower()).Replace(@"\*", ".*").Replace(@"\?", ".")));
    }

    public static async Task MigrateDbContextAsync<TContext>(this WebApplicationBuilder builder,
        Func<TContext, IServiceProvider, Task> seeder) where TContext : DbContext
    {
        var services = builder.Services.BuildServiceProvider();
        var context = services.GetRequiredService<TContext>();
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }
        await seeder(context, services);
    }
}
