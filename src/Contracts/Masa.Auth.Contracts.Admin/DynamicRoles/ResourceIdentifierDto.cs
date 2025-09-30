// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class ResourceIdentifierDto
{
    public string Service { get; set; } = "*";

    public string Region { get; set; } = "*";

    public string Identifier { get; set; } = "*";

    /// <summary>
    /// 从字符串解析Resource，格式为 Service:Region:Identifier
    /// </summary>
    /// <param name="resource">资源标识符字符串</param>
    public ResourceIdentifierDto(string? resource = null)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            return; // 使用默认值 "*"
        }

        var parts = resource.Split(':', StringSplitOptions.None);

        if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
            Service = parts[0];

        if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            Region = parts[1];

        if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            Identifier = parts[2];
    }

    public override string ToString()
    {
        return $"{Service}:{Region}:{Identifier}";
    }
}

/// <summary>
/// Resource的JSON转换器
/// </summary>
public class ResourceIdentifierConverter : JsonConverter<List<ResourceIdentifierDto>>
{
    public override List<ResourceIdentifierDto> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var resource = reader.GetString();
            return new List<ResourceIdentifierDto> { new ResourceIdentifierDto(resource) };
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            var resources = JsonSerializer.Deserialize<List<string>>(ref reader, options);
            return resources?.ConvertAll(resource => new ResourceIdentifierDto(resource)) ?? new List<ResourceIdentifierDto>();
        }
        throw new JsonException("Unexpected token type.");
    }

    public override void Write(Utf8JsonWriter writer, List<ResourceIdentifierDto> value, JsonSerializerOptions options)
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
            var resources = value?.Select(action => action.ToString()) ?? new List<string>();
            JsonSerializer.Serialize(writer, resources, options);
        }
    }
}
