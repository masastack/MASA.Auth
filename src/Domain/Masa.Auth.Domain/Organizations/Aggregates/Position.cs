// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Organizations.Aggregates;

public class Position : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public Position(string name) => Name = name;

    public void Update(string name)
    {
        Name = name;
    }
}

