// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyUserEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyUser>
{
    public void Configure(EntityTypeBuilder<ThirdPartyUser> builder)
    {
        builder.HasKey(tpu => tpu.Id);
        builder.HasOne(tpu => tpu.User).WithMany().HasForeignKey(tpu => tpu.UserId);
        builder.HasOne(tpu => tpu.ThirdPartyIdp).WithMany().HasForeignKey(tpu => tpu.ThirdPartyIdpId);
        builder.HasOne(tpu => tpu.CreateUser).WithMany().HasForeignKey(tpu => tpu.Creator).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasOne(tpu => tpu.ModifyUser).WithMany().HasForeignKey(tpu => tpu.Modifier).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
    }
}

