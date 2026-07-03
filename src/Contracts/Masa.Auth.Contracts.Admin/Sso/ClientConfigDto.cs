// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientConfigDto
{
    /// <summary>
    /// OIDC client_id
    /// </summary>
    public string ClientId { get; set; } = "";

    /// <summary>
    /// 密码规则（正则表达式）；为空表示回退到全局 DCC 配置
    /// </summary>
    public ClientPasswordRuleDto PasswordRule { get; set; } = new();

    /// <summary>
    /// 密码验证失败提示（i18n key）；为空表示回退到全局 DCC 配置
    /// </summary>
    /// <summary>
    /// 短信场景 × 渠道 × 模板配置
    /// </summary>
    public List<ClientSmsTemplateDto> SmsTemplates { get; set; } = new();

    /// <summary>
    /// 邮件场景 × 渠道 × 模板配置
    /// </summary>
    public List<ClientEmailTemplateDto> EmailTemplates { get; set; } = new();
}
