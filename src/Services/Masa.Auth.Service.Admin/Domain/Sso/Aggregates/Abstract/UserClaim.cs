// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract
{
    public abstract class UserClaim : Entity<int>
    {
        public string Type { get; protected set; } = "";
    }
}
