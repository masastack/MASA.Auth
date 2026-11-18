// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace Masa.Auth.Web.Sso.Infrastructure.Oidc;

/// <summary>
/// IS4 将 JSON claim 存为 <see cref="JObject"/>，IdentityModel 8 序列化时会错误展开；产出前转为 <see cref="JsonElement"/>（如 RFC 8693 act）。
/// </summary>
public class JsonClaimTokenCreationService : DefaultTokenCreationService
{
    public JsonClaimTokenCreationService(
        ISystemClock clock,
        IKeyMaterialService keys,
        IdentityServerOptions options,
        ILogger<DefaultTokenCreationService> logger)
        : base(clock, keys, options, logger)
    {
    }

    protected override async Task<JwtPayload> CreatePayloadAsync(Token token)
    {
        var payload = await base.CreatePayloadAsync(token);

        foreach (var key in payload.Keys.ToList())
        {
            payload[key] = Normalize(payload[key]);
        }

        return payload;
    }

    internal static object Normalize(object value)
    {
        if (value is JToken token)
        {
            return ToJsonElement(token);
        }

        // 多个 JSON claim 时可能是 JObject 集合
        if (value is not string and System.Collections.IEnumerable items)
        {
            var list = items.Cast<object>().ToList();
            if (list.Any(item => item is JToken))
            {
                return list.Select(Normalize).ToList();
            }
        }

        return value;
    }

    static JsonElement ToJsonElement(JToken token)
    {
        using var document = JsonDocument.Parse(token.ToString(Newtonsoft.Json.Formatting.None));
        return document.RootElement.Clone();
    }
}
