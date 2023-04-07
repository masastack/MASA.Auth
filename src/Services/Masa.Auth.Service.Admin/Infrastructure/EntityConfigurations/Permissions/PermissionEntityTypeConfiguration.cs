// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Permissions;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.SystemId, p.AppId, p.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.Property(p => p.Name).HasMaxLength(40).IsRequired();
        builder.Property(p => p.Code).HasMaxLength(255).IsRequired();
        builder.Property(p => p.Url).HasMaxLength(255).IsRequired(false);
        builder.Property(p => p.Icon).IsRequired(false);
        builder.Property(p => p.Description).HasMaxLength(255).IsRequired(false);
        builder.Property(p => p.Type).HasConversion(
            v => v.ToString(),
            v => (PermissionTypes)Enum.Parse(typeof(PermissionTypes), v)
        );
        builder.HasMany(p => p.UserPermissions).WithOne(up => up.Permission).HasForeignKey(up => up.PermissionId);
        builder.HasMany(p => p.RolePermissions).WithOne(rp => rp.Permission).HasForeignKey(rp => rp.PermissionId);
        builder.HasMany(p => p.TeamPermissions).WithOne(tp => tp.Permission).HasForeignKey(tp => tp.PermissionId);
        builder.HasMany(p => p.LeadingPermissions).WithMany(pi => pi.AffiliationPermissions)
            .UsingEntity<PermissionRelation>(
                    configureRight => configureRight
                    .HasOne(pr => pr.LeadingPermission)
                    .WithMany(p => p.AffiliationPermissionRelations)
                    .HasForeignKey(pr => pr.LeadingPermissionId),
                    configureLeft => configureLeft
                    .HasOne(pr => pr.AffiliationPermission)
                    .WithMany(p => p.LeadingPermissionRelations)
                    .HasForeignKey(pr => pr.AffiliationPermissionId),
                    configureJoinEntityType =>
                    {
                        configureJoinEntityType.HasKey(pr => pr.Id);
                        configureJoinEntityType.HasIndex(pr => new { pr.LeadingPermissionId, pr.AffiliationPermissionId })
                            .IsUnique().HasFilter("[IsDeleted] = 0");
                    });
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.ReplenishCode);
    }
}

