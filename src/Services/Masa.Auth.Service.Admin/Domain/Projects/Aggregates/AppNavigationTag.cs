// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Projects.Aggregates;

public class AppNavigationTag : AggregateRoot<Guid>
{
    public string AppIdentity { get; private set; }

    public string Tag { get; private set; }

    public AppNavigationTag(string appIdentity, string tag)
    {
        AppIdentity = appIdentity;
        Tag = tag;
    }

    public void UpdateTag(string tag)
    {
        Tag = tag;
    }
}
