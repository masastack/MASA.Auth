// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class AuthenticationExternalConstants
{
    public static string ExternalCookieAuthenticationScheme { get; } = "oauth.external";

    public static string ChallengeEndpoint { get; } = "external/challenge";

    public static string CallbackEndpoint { get; } = "external/callback";
}
