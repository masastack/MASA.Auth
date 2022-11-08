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

    public static void MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<TContext>();
            try
            {
                context.Database.EnsureCreated();
            }
            catch
            {
            }
            seeder(context, services);
        }
    }
}
