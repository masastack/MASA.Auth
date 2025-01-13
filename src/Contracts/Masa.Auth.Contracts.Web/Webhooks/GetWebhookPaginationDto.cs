// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Webhooks;

public class GetWebhookPaginationDto : Pagination
{
    public string Name { get; set; } = string.Empty;
}
