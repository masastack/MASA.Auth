// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

/// <summary>
/// StatementEffect的JSON转换器，用于序列化和反序列化
/// </summary>
public class StatementEffectConverter : JsonConverter<StatementEffect>
{
    public override StatementEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ParseEffect(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, StatementEffect value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    /// <summary>
    /// 解析Effect字段，支持多种格式：
    /// - 数字：0=Deny, 1=Allow
    /// - 字符串枚举："Allow", "Deny" (大小写不敏感)
    /// - 字符串数字："0"=Deny, "1"=Allow
    /// - 布尔字符串："true"=Allow, "false"=Deny (大小写不敏感)
    /// </summary>
    private static StatementEffect ParseEffect(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                // 支持数字格式：0=Deny, 1=Allow
                var numberValue = reader.GetInt32();
                return numberValue switch
                {
                    0 => StatementEffect.Deny,
                    1 => StatementEffect.Allow,
                    _ => StatementEffect.Deny // 默认拒绝
                };

            case JsonTokenType.String:
                var stringValue = reader.GetString()?.Trim();
                if (string.IsNullOrEmpty(stringValue))
                    return StatementEffect.Deny;

                // 1. 尝试解析枚举名称 (Allow/Deny)
                if (Enum.TryParse<StatementEffect>(stringValue, true, out var enumResult))
                {
                    return enumResult;
                }

                // 2. 尝试解析数字字符串 ("0"/"1")
                if (int.TryParse(stringValue, out var intValue))
                {
                    return intValue switch
                    {
                        0 => StatementEffect.Deny,
                        1 => StatementEffect.Allow,
                        _ => StatementEffect.Deny
                    };
                }

                // 3. 尝试解析布尔字符串 ("true"/"false")
                if (bool.TryParse(stringValue, out var boolValue))
                {
                    return boolValue ? StatementEffect.Allow : StatementEffect.Deny;
                }

                // 4. 其他特殊字符串处理
                return stringValue.ToLowerInvariant() switch
                {
                    "yes" => StatementEffect.Allow,
                    "no" => StatementEffect.Deny,
                    _ => StatementEffect.Deny // 默认拒绝
                };

            case JsonTokenType.True:
                // 支持JSON布尔值 true
                return StatementEffect.Allow;

            case JsonTokenType.False:
                // 支持JSON布尔值 false
                return StatementEffect.Deny;

            default:
                // 其他类型默认拒绝
                return StatementEffect.Deny;
        }
    }
}

