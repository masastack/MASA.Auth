// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer;

public class AuthSqlServerDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        AuthDbContext.RegisterAssembly(typeof(AuthSqlServerDbContextFactory).Assembly);
        var optionsBuilder = new MasaDbContextOptionsBuilder<AuthDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection")!, b => b.MigrationsAssembly("Masa.Auth.EntityFrameworkCore.SqlServer"));

        return new AuthDbContext(optionsBuilder.MasaOptions);
    }
}
