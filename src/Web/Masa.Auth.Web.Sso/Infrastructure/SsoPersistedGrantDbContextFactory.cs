// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class SsoPersistedGrantDbContextFactory : IDesignTimeDbContextFactory<SsoPersistedGrantDbContext>
{
    public SsoPersistedGrantDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<SsoPersistedGrantDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        var storeOptions = new OperationalStoreOptions();
        return new SsoPersistedGrantDbContext(optionsBuilder.Options, storeOptions);
    }
}