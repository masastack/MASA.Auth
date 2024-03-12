// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

[Serializable]
public class ImpersonationCacheItem
{
    public const string CACHE_NAME = "AppImpersonationCache";

    public int? ImpersonatorTenantId { get; set; }

    public long ImpersonatorUserId { get; set; }

    public int? TargetTenantId { get; set; }

    public long TargetUserId { get; set; }

    public bool IsBackToImpersonator { get; set; }

    public ImpersonationCacheItem()
    {

    }

    public ImpersonationCacheItem(int? targetTenantId, long targetUserId, bool isBackToImpersonator)
    {
        TargetTenantId = targetTenantId;
        TargetUserId = targetUserId;
        IsBackToImpersonator = isBackToImpersonator;
    }
}