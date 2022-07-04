// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
        .EnableDetailedErrors();

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.HasDefaultSchema("auth");

        builder.Entity<IdentityProvider>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<ThirdPartyIdp>("ThirdParty")
            .HasValue<LdapIdp>("LDAP");

        //foreach (var entityType in builder.Model.GetEntityTypes())
        //{
        //    if (!entityType.ClrType.IsAssignableTo(typeof(IdentityProvider)))
        //    {
        //        entityType.SetTableName(entityType.ClrType.Name.Pluralize());
        //    }
        //}

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyConfigurationsFromAssembly(typeof(UserClaimRepository).Assembly);

        base.OnModelCreatingExecuting(builder);
    }
}