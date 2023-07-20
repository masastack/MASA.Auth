// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Constants;

public static class DefaultUserAttributes
{
    public const string Password = "masa1234";
    public const string MaleAvatar = "https://cdn.masastack.com/stack/images/avatar/mr.chen.svg";
    public const string FemaleAvatar = "https://cdn.masastack.com/stack/images/avatar/ms.qu.svg";

    public static string GetDefaultAvatar(GenderTypes gender)
    {
        if (gender == GenderTypes.Male) return MaleAvatar;
        if (gender == GenderTypes.Female) return FemaleAvatar;
        return MaleAvatar;
    }
}
