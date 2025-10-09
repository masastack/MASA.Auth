// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class ActionIdentifierDto
{
    [JsonPropertyName("Resource")]
    public string Resource { get; set; } = "*";

    [JsonPropertyName("Type")]
    public string Type { get; set; } = "*";

    [JsonPropertyName("Operation")]
    public string Operation { get; set; } = "*";

    /// <summary>
    /// 无参构造函数，用于JSON反序列化
    /// </summary>
    public ActionIdentifierDto()
    {
    }

    /// <summary>
    /// 从字符串解析ActionIdentifier，格式为 Resource:Type:Operation
    /// </summary>
    /// <param name="actionName">操作标识符字符串</param>
    public ActionIdentifierDto(string? actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return; // 使用默认值 "*"
        }

        var parts = actionName.Split(':', StringSplitOptions.None);

        if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
            Resource = parts[0];

        if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            Type = parts[1];

        if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            Operation = parts[2];
    }

    public override string ToString()
    {
        return $"{Resource}:{Type}:{Operation}";
    }
}

/// <summary>
/// ActionIdentifier的JSON转换器
/// </summary>
public class ActionIdentifierConverter : JsonConverter<List<ActionIdentifierDto>>
{
    public override List<ActionIdentifierDto> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var actionName = reader.GetString();
            return new List<ActionIdentifierDto> { new ActionIdentifierDto(actionName) };
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            var result = new List<ActionIdentifierDto>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.String)
                {
                    var actionName = reader.GetString();
                    result.Add(new ActionIdentifierDto(actionName));
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var actionObj = JsonSerializer.Deserialize<ActionIdentifierDto>(ref reader, options);
                    if (actionObj != null)
                        result.Add(actionObj);
                }
            }
            return result;
        }
        throw new JsonException("Unexpected token type.");
    }

    public override void Write(Utf8JsonWriter writer, List<ActionIdentifierDto> value, JsonSerializerOptions options)
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteStringValue("*");
        }
        else if (value.Count == 1)
        {
            writer.WriteStringValue(value[0].ToString());
        }
        else
        {
            var actionNames = value?.Select(action => action.ToString()) ?? new List<string>();
            JsonSerializer.Serialize(writer, actionNames, options);
        }
    }
}
