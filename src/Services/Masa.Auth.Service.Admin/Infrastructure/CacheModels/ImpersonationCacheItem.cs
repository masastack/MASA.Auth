// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

[Serializable]
public class ImpersonationCacheItem
{
    public Guid ImpersonatorUserId { get; set; }

    public Guid TargetUserId { get; set; }

    public bool IsBackToImpersonator { get; set; }

    public ImpersonationCacheItem()
    {

    }

    public ImpersonationCacheItem(Guid targetUserId, bool isBackToImpersonator)
    {
        TargetUserId = targetUserId;
        IsBackToImpersonator = isBackToImpersonator;
    }
}