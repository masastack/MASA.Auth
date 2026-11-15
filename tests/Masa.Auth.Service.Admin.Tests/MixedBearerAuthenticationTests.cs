// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityServer4.Services;
using Masa.Auth.Service.Admin.Infrastructure.Auth;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 令牌形状分流（Mixed Bearer 校验的 PolicyScheme 路由核心）：
/// JWT（三段非空 '.' 分段）→ 本地验签；reference token（opaque 句柄）→ SSO introspection；
/// 两者都不像的垃圾凭据 → 走 jwt 在本地 401，**不向 SSO 发 introspection 请求**（introspection 默认不缓存，
/// 若任何非 JWT 形状都转发，匿名调用方就能放大对 SSO 的请求）。
/// 分流错了会把 Reference 客户端打死（JWT 解析不了 opaque / 句柄被误判为垃圾）或白白增加 introspection 往返，所以锚定行为。
/// </summary>
public class MixedBearerAuthenticationTests
{
    const string Jwt = AuthSchemes.Jwt;
    const string Introspection = AuthSchemes.Introspection;

    const string RealJwt =
        "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEifQ.eyJzdWIiOiIxMjM0NTY3ODkwIn0.SIGNATURE";

    // IS4 默认句柄的真实形态：32 字节随机数 base64url（43 字符，无 '.' 无填充）
    const string Is4Handle = "kQ3x9mT2vLw8Zr4bN7cYpA1sDfGhJ5uE0iOaXyMq-_U";

    [Fact]
    public void Resolve_RoutesJwtToLocalValidation()
    {
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {RealJwt}"));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"bearer {RealJwt}"));   // scheme 前缀大小写不敏感
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"BEARER  {RealJwt}".Replace("  ", " ")));
    }

    [Fact]
    public void Resolve_RoutesOpaqueReferenceTokenToIntrospection()
    {
        Assert.Equal(43, Is4Handle.Length);
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"Bearer {Is4Handle}"));
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"bearer  {Is4Handle}  "));   // 前缀大小写 / 首尾空白不敏感
        // 自定义 IHandleGenerationService 常见形态：GUID（带 '-'）、hex
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve("Bearer 8f2c7b1a-93d4-4e5f-a06b-2c3d4e5f6a7b"));
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"Bearer {new string('a', 64)}"));
    }

    /// <summary>形状规则必须对着 IS4 真实的句柄生成器：规则写严了，Reference 客户端会被本地 401</summary>
    [Fact]
    public async Task Resolve_AcceptsEveryHandleIs4ActuallyGenerates()
    {
        // 经接口调用：IS4 运行时（DefaultGrantStore）就是这样取接口的默认长度参数
        IHandleGenerationService generator = new DefaultHandleGenerationService();

        for (var i = 0; i < 500; i++)
        {
            var handle = await generator.GenerateAsync();

            Assert.True(TokenSchemeSelector.LooksLikeReferenceHandle(handle), $"IS4 handle rejected: {handle}");
            Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"Bearer {handle}"));
        }
    }

    [Fact]
    public void Resolve_DefaultsToJwt_WhenNoBearerCredential()
    {
        // 无凭据走 jwt：与历史行为一致（401），且不向 SSO 发多余请求
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve(null));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve(""));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve("   "));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve("Basic dXNlcjpwYXNz"));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve("Negotiate someblob"));
    }

    [Theory]
    [InlineData("a..c")]                                     // 空段的畸形 JWT
    [InlineData(".b.")]
    [InlineData("twoparts.only")]
    [InlineData("four.parts.a.b")]
    [InlineData("")]                                         // "Bearer " 后没有令牌
    [InlineData("short")]                                    // 过短：不可能是句柄
    public void Resolve_MalformedOrImplausibleBearer_IsRejectedLocally_NotForwardedToSso(string token)
    {
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {token}"));
    }

    [Theory]
    [InlineData("has space inside-aaaaaaaaaaaaaaaaaaaaaaaaaaaa")]   // 空白
    [InlineData("contains+plus/slash==aaaaaaaaaaaaaaaaaaaaaa")]     // 标准 base64 字符（+ / =）不是 base64url
    [InlineData("非ascii令牌非ascii令牌非ascii令牌非ascii令牌")]       // 非 ASCII
    [InlineData("aaaaaaaa.bbbbbbbb.cccccccc.dddddddd.eeeeeeee")]    // JWE 五段，不是本系统签发的形态
    public void Resolve_BadCharacters_AreRejectedLocally(string token)
    {
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {token}"));
    }

    [Fact]
    public void Resolve_HandleLengthBounds_AreEnforced()
    {
        var min = TokenSchemeSelector.MinReferenceHandleLength;
        var max = TokenSchemeSelector.MaxReferenceHandleLength;

        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {new string('a', min - 1)}"));
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"Bearer {new string('a', min)}"));
        Assert.Equal(Introspection, TokenSchemeSelector.Resolve($"Bearer {new string('a', max)}"));
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {new string('a', max + 1)}"));
        // 超长垃圾（例如被塞了整页内容的 Authorization 头）不能被转发
        Assert.Equal(Jwt, TokenSchemeSelector.Resolve($"Bearer {new string('a', 10_000)}"));
    }

    [Fact]
    public void ReferenceHandle_AndJwtShapes_AreMutuallyExclusive()
    {
        // 句柄字符集不含 '.'，所以任何 JWT 形状都不可能同时被当成句柄
        Assert.True(TokenSchemeSelector.LooksLikeJwt(RealJwt));
        Assert.False(TokenSchemeSelector.LooksLikeReferenceHandle(RealJwt));
        Assert.False(TokenSchemeSelector.LooksLikeJwt(Is4Handle));
        Assert.True(TokenSchemeSelector.LooksLikeReferenceHandle(Is4Handle));
    }

    [Fact]
    public void SchemeNames_KeepDefaultSchemeBackwardCompatible()
    {
        // 对外默认 scheme 名不变：[Authorize] / 挑战头 / 其他显式引用不受接线改造影响
        Assert.Equal("Bearer", AuthSchemes.Bearer);
    }
}
