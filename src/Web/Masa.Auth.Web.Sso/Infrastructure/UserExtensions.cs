// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public static class UserExtensions
{
    public static Claim[] GetUserClaims(this UserModel user) => new Claim[]
    {
        new Claim("account", user.Account),
        new Claim("username", user.DisplayName),
        new Claim("phoneNumber", user.PhoneNumber!),
    };
}
