// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class ColorPairHelper
{
    static readonly Dictionary<string, ColorPair> _colorGroup = new Dictionary<string, ColorPair>
    {
        { "purple", new("#4318FF","#F4F1FF") },
        { "green",  new("#05CD99","#E6FAF5") },
        { "red",  new("#FF5252","#FFECE8") },
        { "blue",  new("#37A7FF","#EBF6FF") },
        { "orange", new("#FF7D00","#FFF7E8") }
    };

    public static ColorPair GetColorGroup(string color)
    {
        _colorGroup.TryGetValue(color, out var colorGroup);
        return colorGroup ?? _colorGroup.First().Value;
    }
}
