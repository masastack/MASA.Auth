// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Masa.Auth.EntityFrameworkCore.PostgreSql.Converters;

public class NullableDateTimeUtcConverter : ValueConverter<DateTime?, DateTime?>
{
    /// <summary>
    /// 初始化 NullableDateTimeUtcConverter 的新实例。
    /// </summary>
    public NullableDateTimeUtcConverter() : base(
        v => v.HasValue ? v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc).ToUniversalTime() : v.Value.ToUniversalTime() : null,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null)
    {
    }
}
