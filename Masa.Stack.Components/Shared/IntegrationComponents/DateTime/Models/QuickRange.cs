// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components
{
    public class QuickRange
    {
        public QuickRangeKey Key { get; init; }

        public TimeSpan? Relative { get; init; }

        public bool Disabled { get; init; }

        public QuickRange(QuickRangeKey key)
        {
            Key = key;
        }

        public QuickRange(QuickRangeKey key, TimeSpan relative) : this(key)
        {
            Relative = relative;
        }

        public QuickRange(QuickRangeKey key, TimeSpan relative, bool disabled) : this(key, relative)
        {
            Disabled = disabled;
        }

        public static QuickRange DefaultQuickRange => new(QuickRangeKey.Last12Hours, TimeSpan.FromHours(-12));

        public static List<QuickRange> DefaultQuickRanges =>
            new()
            {
                new(QuickRangeKey.Last5Minutes, TimeSpan.FromMinutes(-5)),
                new(QuickRangeKey.Last15Minutes, TimeSpan.FromMinutes(-15)),
                new(QuickRangeKey.Last30Minutes, TimeSpan.FromMinutes(-30)),
                new(QuickRangeKey.Last1Hour, TimeSpan.FromHours(-1)),
                new(QuickRangeKey.Last3Hours, TimeSpan.FromHours(-3)),
                new(QuickRangeKey.Last6Hours, TimeSpan.FromHours(-6)),
                DefaultQuickRange,
                new(QuickRangeKey.Last24Hours, TimeSpan.FromHours(-24)),
                new(QuickRangeKey.Last2Days, TimeSpan.FromDays(-2)),
                new(QuickRangeKey.Last7Days, TimeSpan.FromDays(-7)),
                new(QuickRangeKey.Last30Days, TimeSpan.FromDays(-30)),
            };

        public bool TryGetRange(TimeSpan offset, out (DateTimeOffset start, DateTimeOffset end) value)
        {
            var utcNow = DateTimeOffset.UtcNow;
            value = (utcNow, utcNow);

            if (Relative.HasValue)
            {
                var end = utcNow.ToOffset(offset);
                var start = end.Add(Relative.Value);
                value = (start, end);
                return true;
            }

            switch (Key)
            {
                case QuickRangeKey.Yesterday:
                    {
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-1)).ToOffset(offset);
                        var end = start.AddDays(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.DayBeforeYesterday:
                    {
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-2)).ToOffset(offset);
                        var end = start.AddDays(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.DayLastWeek:
                    {
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-7)).ToOffset(offset);
                        var end = start.AddDays(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.LastWeek:
                    {
                        var dayOfWeek = (int)utcNow.DayOfWeek;
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-7 - dayOfWeek + 1)).ToOffset(offset);
                        var end = start.AddDays(7).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.LastMonth:
                    {
                        var todayLastMonth = utcNow.AddMonths(-1);
                        var start = new DateTimeOffset(todayLastMonth.Year, todayLastMonth.Month, 1, 0, 0, 0, offset);
                        var end = start.AddMonths(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.LastQuarter:
                    {
                        var todayLastQuarter = utcNow.AddMonths(-3);
                        var quarter = (int)Math.Ceiling(todayLastQuarter.Month / 3d);
                        var start = new DateTimeOffset(todayLastQuarter.Year, (quarter - 1) * 3 + 1, 1, 0, 0, 0, offset);
                        var end = start.AddMonths(3).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.LastYear:
                    {
                        var todayLastYear = utcNow.AddYears(-1);
                        var start = new DateTimeOffset(todayLastYear.Year, 1, 1, 0, 0, 0, offset);
                        var end = start.AddYears(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.Today:
                    {
                        var start = new DateTimeOffset(utcNow.Date, offset);
                        var end = start.AddDays(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.TodaySoFar:
                    {
                        var start = new DateTimeOffset(utcNow.Date, offset);
                        var end = utcNow.ToOffset(offset);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisWeek:
                    {
                        var dayOfWeek = (int)utcNow.DayOfWeek;
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-dayOfWeek + 1)).ToOffset(offset);
                        var end = start.AddDays(7).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisWeekSoFar:
                    {
                        var dayOfWeek = (int)utcNow.DayOfWeek;
                        var start = new DateTimeOffset(utcNow.Date.AddDays(-dayOfWeek + 1)).ToOffset(offset);
                        var end = utcNow.ToOffset(offset);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisMonth:
                    {
                        var start = new DateTimeOffset(utcNow.Year, utcNow.Month, 1, 0, 0, 0, offset);
                        var end = start.AddMonths(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisMonthSoFar:
                    {
                        var start = new DateTimeOffset(utcNow.Year, utcNow.Month, 1, 0, 0, 0, offset);
                        var end = utcNow.ToOffset(offset);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisYear:
                    {
                        var start = new DateTimeOffset(utcNow.Year, 1, 1, 0, 0, 0, offset);
                        var end = start.AddYears(1).AddMilliseconds(-1);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.ThisYearSoFar:
                    {
                        var start = new DateTimeOffset(utcNow.Year, 1, 1, 0, 0, 0, offset);
                        var end = utcNow.ToOffset(offset);
                        value = (start, end);
                        return true;
                    }
                case QuickRangeKey.Off:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Key));
            }
        }
    }
}
