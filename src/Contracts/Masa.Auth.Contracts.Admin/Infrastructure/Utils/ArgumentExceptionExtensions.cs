// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class ArgumentExceptionExtensions
{
    public static T ThrowIfDefault<T>([NotNull] T? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument is null || argument.Equals(default(T)))
        {
            throw new UserFriendlyException($"Please provider {paramName},{paramName} is required");
        }
        return argument;
    }

    public static string ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new UserFriendlyException($"Please provider {paramName},{paramName} is required");
        }
        return argument;
    }
}
