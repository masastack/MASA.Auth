// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;

namespace Masa.Auth.Web.Sso.Infrastructure.OnlineUser;

/// <summary>
/// 把踢人水位线挂到门户 Cookie 的校验管道上。
///
/// 为什么用 PostConfigure 而不是 <c>AddCookie</c>：IS4 用
/// <c>AddCookie(IdentityServerConstants.DefaultCookieAuthenticationScheme)</c> 注册门户 Cookie，
/// 直接再 AddCookie 会覆盖原有配置（丢失 IS4 的 ExpireTimeSpan / SlidingExpiration 等），
/// 所以只能在其 options 最终化之后追加事件，并保留原有 <c>OnValidatePrincipal</c>。
/// </summary>
public static class SsoKickWatermarkExtensions
{
    public static IServiceCollection AddSsoKickWatermark(this IServiceCollection services)
    {
        services.PostConfigure<CookieAuthenticationOptions>(
            IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
            {
                var original = options.Events.OnValidatePrincipal;
                options.Events.OnValidatePrincipal = async context =>
                {
                    if (original != null)
                        await original(context);

                    // 签发时刻早于 online:kicked-at:* 的 Cookie 一律拒绝（语义见 SsoKickWatermark）
                    await SsoKickWatermark.RejectKickedSessionAsync(context);
                };
            });

        return services;
    }
}
