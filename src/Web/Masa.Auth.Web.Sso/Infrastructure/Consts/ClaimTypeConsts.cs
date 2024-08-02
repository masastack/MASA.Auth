// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Consts;

public static class ClaimTypeConsts
{
    public const string IMPERSONATOR_USER_ID = $"{DEFAULT_PREFIX}/impersonatorUserId";

    public const string DOMAIN_NAME = $"{DEFAULT_PREFIX}/domainName";

    private const string DEFAULT_PREFIX = "https://masastack.com/security/identity/claims";
}
