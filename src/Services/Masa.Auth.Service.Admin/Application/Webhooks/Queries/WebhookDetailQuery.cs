// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Webhooks.Queries;

public record WebhookDetailQuery(Guid Id) : Query<WebhookDetailDto>
{
    public override WebhookDetailDto Result { get; set; } = new();
}
