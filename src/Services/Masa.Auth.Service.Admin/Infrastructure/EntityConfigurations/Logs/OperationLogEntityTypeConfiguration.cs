// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Subjects;

public class OperationLogEntityTypeConfiguration : IEntityTypeConfiguration<OperationLog>
{
    public void Configure(EntityTypeBuilder<OperationLog> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Operator);
        builder.HasIndex(p => p.OperationTime);
        builder.HasIndex(p => p.OperationType);
        builder.HasIndex(p => p.OperationDescription);
    }
}
