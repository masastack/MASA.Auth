// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.DynamicRoles;

public class DynamicRoleEntityTypeConfiguration : IEntityTypeConfiguration<DynamicRole>
{
    public void Configure(EntityTypeBuilder<DynamicRole> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(128);
        builder.Property(x => x.Code).HasMaxLength(128);
        builder.Property(x => x.ClientId).HasMaxLength(128);
        builder.Property(x => x.Description).HasMaxLength(512);
        builder.OwnsMany(x => x.Conditions, b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property(x => x.FieldName).HasMaxLength(128);
            b.Property(x => x.Value);
            b.Property(x => x.OperatorType).HasConversion(v => v.Id, v => Enumeration.FromValue<OperatorType>(v));
            b.Property(x => x.DataType).HasConversion(v => v.Id, v => Enumeration.FromValue<DynamicRoleDataType>(v));
            b.HasKey("Id");
        });
        builder.OwnsMany(x => x.ControlPolicies, b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd();
            b.Property(x => x.Name).HasMaxLength(128);
            b.Property(x => x.Effect).HasConversion(v => v.Id, v => Enumeration.FromValue<StatementEffect>(v));
            b.HasKey("Id");

            // 配置Actions集合 - 作为独立表存储
            b.OwnsMany(cp => cp.Actions, ab =>
            {
                ab.Property<Guid>("Id").ValueGeneratedOnAdd();
                ab.Property(a => a.Resource).HasMaxLength(128).IsRequired();
                ab.Property(a => a.Type).HasMaxLength(128).IsRequired();
                ab.Property(a => a.Operation).HasMaxLength(128).IsRequired();
                ab.HasKey("Id");
            });

            // 配置Resources集合 - 作为独立表存储
            b.OwnsMany(cp => cp.Resources, rb =>
            {
                rb.Property<Guid>("Id").ValueGeneratedOnAdd();
                rb.Property(r => r.Service).HasMaxLength(128).IsRequired();
                rb.Property(r => r.Region).HasMaxLength(128).IsRequired();
                rb.Property(r => r.Identifier).HasMaxLength(128).IsRequired();
                rb.HasKey("Id");
            });
        });
    }
}
