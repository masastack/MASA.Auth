// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Service.Admin.Infrastructure.ValueConverters;

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class ThirdPartyUserEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyUser>
{
    public void Configure(EntityTypeBuilder<ThirdPartyUser> builder)
    {
        builder.HasKey(tpu => tpu.Id);
        builder.HasOne(tpu => tpu.User).WithMany().HasForeignKey(tpu => tpu.UserId);
        builder.HasOne(tpu => tpu.IdentityProvider).WithMany().HasForeignKey(tpu => tpu.ThirdPartyIdpId);
        builder.HasIndex(u => new { u.CreationTime, u.ModificationTime });//.IsDescending(); supported 7.0
        builder.Navigation(tpu => tpu.IdentityProvider).AutoInclude();
        builder.Property(tpu => tpu.ClaimData).HasConversion(new JsonValueConverter<Dictionary<string, string>>());
    }
}

