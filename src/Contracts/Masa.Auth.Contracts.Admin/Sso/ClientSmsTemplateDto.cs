// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

/// <summary>
/// Client 级短信模板配置：一个发送场景对应一个渠道 + 模板
/// </summary>
public class ClientSmsTemplateDto
{
    public SendMsgCodeTypes Scene { get; set; }

    /// <summary>
    /// MC 渠道编码
    /// </summary>
    public string ChannelCode { get; set; } = "";

    /// <summary>
    /// MC 模板编码
    /// </summary>
    public string TemplateCode { get; set; } = "";
}
