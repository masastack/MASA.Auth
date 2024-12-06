// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.EntityFrameworkCore.SqlServer;

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseDbSql(this MasaDbContextBuilder builder, string dbType)
    {
        switch (dbType)
        {
            case "PostgreSql":
                AuthDbContext.RegisterAssembly(typeof(AuthPostgreSqlDbContextFactory).Assembly);
                builder.UseNpgsql(b => b.MigrationsAssembly("Masa.Auth.EntityFrameworkCore.PostgreSql"));
                break;
            default:
                AuthDbContext.RegisterAssembly(typeof(AuthSqlServerDbContextFactory).Assembly);
                builder.UseSqlServer(b => b.MigrationsAssembly("Masa.Auth.EntityFrameworkCore.SqlServer"));
                break;
        }
        return builder;
    }
}
