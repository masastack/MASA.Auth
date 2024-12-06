// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Sso;

public class RegisterFieldEntityTypeConfiguration : IEntityTypeConfiguration<RegisterField>
{
    public void Configure(EntityTypeBuilder<RegisterField> builder)
    {
        builder.HasKey(registerField => registerField.Id);
    }
}
