// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Text.Json;
using Masa.BuildingBlocks.StackSdks.Mc.Enum;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.EntityConfigurations.Sso;

public class ClientConfigEntityTypeConfiguration : IEntityTypeConfiguration<ClientConfig>
{
    public void Configure(EntityTypeBuilder<ClientConfig> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClientId).HasMaxLength(200).IsRequired();
        // Soft delete (IsDeleted) leaves the row in place, so the unique index must exclude
        // deleted rows - otherwise a client can never be reconfigured after its config is removed.
        builder.HasIndex(x => x.ClientId).IsUnique().HasFilter("NOT \"IsDeleted\"");
        builder.Property(x => x.PasswordRule).HasMaxLength(500);
        builder.Property(x => x.PasswordPrompt).HasMaxLength(500);
        builder.Property(x => x.PasswordRuleConfig)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? null
                    : JsonSerializer.Deserialize<ClientPasswordRuleConfig>(v, (JsonSerializerOptions?)null))
            .Metadata.SetValueComparer(new ValueComparer<ClientPasswordRuleConfig?>(
                (l, r) => JsonSerializer.Serialize(l, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(r, (JsonSerializerOptions?)null),
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                v => v == null ? null : v.Clone()));
        builder.Property(x => x.PasswordRuleConfig).HasMaxLength(2000);
        builder.HasMany(x => x.MessageTemplates).WithOne().HasForeignKey(x => x.ClientConfigId);
    }
}

public class ClientMessageTemplateEntityTypeConfiguration : IEntityTypeConfiguration<ClientMessageTemplate>
{
    public void Configure(EntityTypeBuilder<ClientMessageTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChannelCode).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TemplateCode).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.ClientConfigId, x.ChannelType, x.Scene }).IsUnique();
    }
}