// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.EntityConfigurations.Webhooks;

public class WebhookEntityTypeConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.WebhookEvent).HasConversion(
            v => v.ToString(),
            v => (WebhookEvent)Enum.Parse(typeof(WebhookEvent), v)
        );
        builder.HasMany(p => p.WebhookLogs).WithOne(up => up.Webhook).HasForeignKey(up => up.WebhookId);
    }
}
