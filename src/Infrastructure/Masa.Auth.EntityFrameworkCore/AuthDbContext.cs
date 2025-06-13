// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore;

public class AuthDbContext : MasaDbContext<AuthDbContext>
{
    internal static List<Assembly> Assemblies = new() { typeof(AuthDbContext).Assembly };

    public AuthDbContext(MasaDbContextOptions<AuthDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.DbContextOptionsBuilder
        //.LogTo(Console.WriteLine, LogLevel.Warning)
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

        foreach (var assembly in Assemblies.Distinct())
        {
            builder.ApplyConfigurationsFromAssembly(assembly);
        }

        // Apply provider-specific configurations
        if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new DateTimeUtcConverter());
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new NullableDateTimeUtcConverter());
                    }
                }
            }
        }

        base.OnModelCreatingExecuting(builder);
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        if (!Assemblies.Contains(assembly))
        {
            Assemblies.Add(assembly);
        }
    }
}