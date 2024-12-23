// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveApiScopeDto
{
    public Guid Id { get; set; }

    public RemoveApiScopeDto(Guid id)
    {
        Id = id;
    }
}

