// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class Extensions
{
    public static bool WildCardContains(this IEnumerable<string> data, string code)
    {
        return data.Any(item => Regex.IsMatch(code.ToLower(),
            Regex.Escape(item.ToLower()).Replace(@"\*", ".*").Replace(@"\?", ".")));
    }
}
