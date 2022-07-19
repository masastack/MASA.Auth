// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserSystemBusinessData : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; private set; }

    public string Data { get; private set; }

    public string SystemId { get; private set; }

    public UserSystemBusinessData(Guid userId, string systemId, string data)
    {
        UserId = userId;
        Data = data;
        SystemId = systemId;
    }

    public void Update(string data)
    {
        Data = data;
    }
}
