// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Webhooks;

public class WebhookItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Event { get; set; } = "";

    public string Url { get; set; } = "";

    public DateTime CreationTime { get; set; }

    public bool IsActive { get; set; }
}
