// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class AuthDbContext : MasaDbContext<AuthDbContext>
{
    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.DbContextOptionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Warning)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.HasDefaultSchema("auth");

        builder.Entity<IdentityProvider>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<ThirdPartyIdp>("ThirdParty")
            .HasValue<LdapIdp>("LDAP");

        builder.Entity<SubjectPermissionRelation>()
            .HasDiscriminator<string>("_businessType")
            .HasValue<UserPermission>("User")
            .HasValue<RolePermission>("Role")
            .HasValue<TeamPermission>("Team");

        builder.ApplyConfiguration(new IntegrationEventLogEntityTypeConfiguration());
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()!);

        builder.ApplyConfigurationsFromAssembly(typeof(UserClaimRepository).Assembly);

        base.OnModelCreatingExecuting(builder);
    }
}
