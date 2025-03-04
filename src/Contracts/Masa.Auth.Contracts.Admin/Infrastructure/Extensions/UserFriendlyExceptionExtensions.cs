﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Extensions;

public static class UserFriendlyExceptionExtensions
{
    public static void ThrowIfEmpty(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.VALUE_IS_EMPTY, nameof(value));
        }
    }
}
