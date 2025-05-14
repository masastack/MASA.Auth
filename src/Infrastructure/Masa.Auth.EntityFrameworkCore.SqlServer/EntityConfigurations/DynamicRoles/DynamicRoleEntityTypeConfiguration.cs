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
            b.Property(x => x.Value).HasMaxLength(512);
            b.Property(x => x.OperatorType).HasConversion(v => v.Id, v => Enumeration.FromValue<OperatorType>(v));
            b.Property(x => x.DataType).HasConversion(v => v.Id, v => Enumeration.FromValue<DynamicRoleDataType>(v));
            b.HasKey("Id");
        });
    }
}
