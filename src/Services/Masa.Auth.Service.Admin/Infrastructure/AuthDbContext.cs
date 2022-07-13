// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Design;

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

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new MasaDbContextOptionsBuilder<AuthDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new AuthDbContext(optionsBuilder.MasaOptions);
    }
}