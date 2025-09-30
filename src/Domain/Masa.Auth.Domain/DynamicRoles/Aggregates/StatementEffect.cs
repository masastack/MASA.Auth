// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class StatementEffect : Enumeration
{
    public static StatementEffect Deny = new StatementEffect(0, "Deny");

    public static StatementEffect Allow = new StatementEffect(1, "Allow");

    private StatementEffect(int id, string name) : base(id, name)
    {
    }
}
