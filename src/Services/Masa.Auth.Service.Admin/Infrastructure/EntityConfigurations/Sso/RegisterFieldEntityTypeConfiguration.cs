// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Sso;

public class RegisterFieldEntityTypeConfiguration : IEntityTypeConfiguration<RegisterField>
{
    public void Configure(EntityTypeBuilder<RegisterField> builder)
    {
        builder.ToTable(nameof(RegisterField), AuthDbContext.SSO_SCHEMA).HasKey(registerField => registerField.Id);
    }
}
