// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public record IntervalItem(string Text, TimeSpan TimeSpan)
{
    public static readonly IntervalItem Off = new("Off", TimeSpan.Zero);
    public static readonly IntervalItem TenSecond = new("10s", TimeSpan.FromSeconds(10));
    public static readonly IntervalItem ThirtySecond = new("30s", TimeSpan.FromSeconds(30));
    public static readonly IntervalItem OneMinute = new("1m", TimeSpan.FromMinutes(1));
    public static readonly IntervalItem FiveMinute = new("5m", TimeSpan.FromMinutes(5));
    public static readonly IntervalItem FifteenMinute = new("15m", TimeSpan.FromMinutes(15));
    public static readonly IntervalItem ThirtyMinute = new("30m", TimeSpan.FromMinutes(30));
    public static readonly IntervalItem OneHour = new("1h", TimeSpan.FromHours(1));
    public static readonly IntervalItem TwoHour = new("2h", TimeSpan.FromHours(2));
    public static readonly IntervalItem OneDay = new("1d", TimeSpan.FromDays(1));

    public static readonly List<IntervalItem> Items = new()
    {
        Off, TenSecond, ThirtySecond, OneMinute, FiveMinute, FifteenMinute, ThirtyMinute, OneHour, TwoHour, OneDay
    };
}