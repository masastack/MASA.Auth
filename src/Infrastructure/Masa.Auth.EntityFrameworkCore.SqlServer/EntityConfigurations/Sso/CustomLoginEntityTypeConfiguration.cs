// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.SqlServer.EntityConfigurations.Sso;

public class CustomLoginEntityTypeConfiguration : IEntityTypeConfiguration<CustomLogin>
{
    public void Configure(EntityTypeBuilder<CustomLogin> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasMany(x => x.ThirdPartyIdps).WithOne().HasForeignKey(x => x.CustomLoginId);
        builder.HasMany(x => x.RegisterFields).WithOne().HasForeignKey(x => x.CustomLoginId);
        builder.HasOne(x => x.CreateUser).WithMany().HasForeignKey(x => x.Creator).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasOne(x => x.ModifyUser).WithMany().HasForeignKey(x => x.Modifier).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
    }
}
