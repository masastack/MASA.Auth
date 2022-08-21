// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global;

public static class CacheKey
{
    const string LOGIN_CODE = "sso_login_code";
    const string CONSENT_RESPONSE = "consent_response";

    public static string GetSmsCodeKey(string phoneNumber)
    {
        return $"{LOGIN_CODE}:{phoneNumber}";
    }

    public static string GetConsentResponseKey(string id)
    {
        return $"{CONSENT_RESPONSE}:{id}";
    }
}
