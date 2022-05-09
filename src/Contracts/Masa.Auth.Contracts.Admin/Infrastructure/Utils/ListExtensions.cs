// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class ListExtensions
{
    public static IList<T> Swap<T>(this IList<T> list, int oldIndex, int newIndex)
    {
        var oldItem = list[oldIndex];
        list[oldIndex] = list[newIndex];
        list[newIndex] = oldItem;
        return list;
    }
}

