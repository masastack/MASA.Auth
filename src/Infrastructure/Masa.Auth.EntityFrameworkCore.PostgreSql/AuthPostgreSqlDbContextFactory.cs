// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Authentication.OpenIdConnect.EFCore.PostgreSql;

namespace Masa.Auth.EntityFrameworkCore;

public class AuthPostgreSqlDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        AuthDbContext.RegisterAssembly(typeof(AuthPostgreSqlDbContextFactory).Assembly);
        AuthDbContext.RegisterAssembly(typeof(OpenIdConnectEFPostgreSql).Assembly);
        var optionsBuilder = new MasaDbContextOptionsBuilder<AuthDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.PostgreSql.json")
            .Build();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!, b => b.MigrationsAssembly("Masa.Auth.EntityFrameworkCore.PostgreSql"));

        return new AuthDbContext(optionsBuilder.MasaOptions);
    }
}
