// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using EFCoreSecondLevelCacheInterceptor;

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContext : IsolationDbContext
{
    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .AddInterceptors(Options.ServiceProvider.GetRequiredService<SecondLevelCacheInterceptor>());

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.HasDefaultSchema("auth");

        builder.Entity<IdentityProvider>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<ThirdPartyIdp>("ThirdParty")
            .HasValue<LdapIdp>("LDAP");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!entityType.ClrType.IsAssignableTo(typeof(IdentityProvider)))
            {
                entityType.SetTableName(entityType.ClrType.Name.Pluralize());
            }
        }

        base.OnModelCreatingExecuting(builder);
    }
}