// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.GlobalNavs.Aggregates;

public class GlobalNavVisible : AuditAggregateRoot<Guid, Guid>
{
    public string AppId { get; private set; }

    public string ClientId { get; private set; }
    
    public bool Visible { get; private set; }

    public GlobalNavVisible(string appId, string clientId, bool visible)
    {
        AppId = appId;
        ClientId = clientId;
        Visible = visible;
    }
}
