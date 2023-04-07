// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new MasaDbContextOptionsBuilder<AuthDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.Development.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new AuthDbContext(optionsBuilder.MasaOptions);
    }
}
