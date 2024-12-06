// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.
namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Webhooks;

public class WebhookLogEntityTypeConfiguration : IEntityTypeConfiguration<WebhookLog>
{
    public void Configure(EntityTypeBuilder<WebhookLog> builder)
    {
        builder.HasKey(p => p.Id);
    }
}
