// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using Masa.BuildingBlocks.StackSdks.Config;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// <see cref="IMasaStackConfig"/> 桩：仅实现 <c>GetSsoDomain()</c> / <c>GetServiceId(Auth)</c> 所需的 <c>MASA_STACK</c>；DispatchProxy 避免随 SDK 接口变动维护手写实现。
/// </summary>
public class StubMasaStackConfig : DispatchProxy
{
    string? _ssoDomain;
    string? _authServiceId;

    /// <param name="ssoDomain">SSO 域名；空表示未配置</param>
    public static IMasaStackConfig WithSsoDomain(string? ssoDomain) => CreateStub(ssoDomain, null);

    /// <param name="authServiceId">Auth 服务的 appId（<c>GetServiceId(Auth)</c>）；空表示未配置</param>
    public static IMasaStackConfig WithAuthServiceId(string? authServiceId) => CreateStub(null, authServiceId);

    static IMasaStackConfig CreateStub(string? ssoDomain, string? authServiceId)
    {
        var proxy = Create<IMasaStackConfig, StubMasaStackConfig>();
        var stub = (StubMasaStackConfig)(object)proxy;
        stub._ssoDomain = ssoDomain;
        stub._authServiceId = authServiceId;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod?.Name == nameof(IMasaStackConfig.GetValue) && Equals(args?[0], "MASA_STACK"))
        {
            if (string.IsNullOrWhiteSpace(_ssoDomain) && string.IsNullOrWhiteSpace(_authServiceId))
            {
                return null;
            }

            // MASA_STACK JSON：[{ "id": "auth", "sso": { "domain": "..." }, "service": { "id": "..." } }]
            var auth = new Dictionary<string, object> { ["id"] = "auth" };
            if (!string.IsNullOrWhiteSpace(_ssoDomain))
            {
                auth["sso"] = new { domain = _ssoDomain };
            }

            if (!string.IsNullOrWhiteSpace(_authServiceId))
            {
                auth["service"] = new { id = _authServiceId };
            }

            return System.Text.Json.JsonSerializer.Serialize(new[] { auth });
        }

        var returnType = targetMethod?.ReturnType;
        return returnType is { IsValueType: true } && returnType != typeof(void)
            ? Activator.CreateInstance(returnType)
            : null;
    }
}
