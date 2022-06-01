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

    public static bool IsDuplicate<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector, [NotNullWhen(true)] out List<TSource>? duplicate)
    {
        var group = list.GroupBy(keySelector).Where(g => g.Count() > 1);
        if (group.Count() > 1)
        {
            duplicate = group.SelectMany(item => item.Select(e => e)).ToList();
            return true;
        }
        duplicate = null;
        return false;
    }

    public static bool IsDuplicate<TSource, TKey>(this IEnumerable<TSource> list, Func<TSource, TKey> keySelector, [NotNullWhen(true)] out List<TKey>? duplicate)
    {
        var group = list.GroupBy(keySelector).Where(g => g.Count() > 1);
        if (group.Count() > 1)
        {
            duplicate = group.Select(item => item.Select(keySelector).First()).ToList();
            return true;
        }
        duplicate = null;
        return false;
    }
}

