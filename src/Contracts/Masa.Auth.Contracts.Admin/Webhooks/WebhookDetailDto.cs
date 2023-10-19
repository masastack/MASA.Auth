// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Webhooks;

public class WebhookDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Url { get; set; } = "";

    public string Description { get; set; } = "";

    public string HttpMethod { get; set; } = "POST";

    public string Secret { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public WebhookEvent Event { get; set; }
}
