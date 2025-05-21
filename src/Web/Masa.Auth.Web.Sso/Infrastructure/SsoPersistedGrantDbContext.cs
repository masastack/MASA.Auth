// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class SsoPersistedGrantDbContext : PersistedGrantDbContext<SsoPersistedGrantDbContext>
{
    public SsoPersistedGrantDbContext(DbContextOptions<SsoPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
        : base(options, storeOptions)
    {
    }
}