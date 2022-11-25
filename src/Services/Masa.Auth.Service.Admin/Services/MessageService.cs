// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class MessageService : ServiceBase
{
    public MessageService() : base("api/message")
    {
        RouteOptions.DisableAutoMapRoute = false;
    }

    [ProductionDisabledRoute]
    public async Task SendSmsAsync(IEventBus eventBus,
        [FromBody] SendMsgCodeModel model)
    {
        await eventBus.PublishAsync(new SendSmsCommand(model));
    }

    [ProductionDisabledRoute]
    public async Task SendEmailAsync(IEventBus eventBus,
        [FromBody] SendEmailModel sendEmailModel)
    {
        await eventBus.PublishAsync(new SendEmailCommand(sendEmailModel));
    }

    protected override string GetMethodName(MethodInfo methodInfo, string prefix, ServiceRouteOptions globalOptions)
    {
        return Regex.Replace(methodInfo.Name.TrimStart(prefix).TrimEnd("Async"), @"(\B[A-Z])", "_$1").ToLower();
    }
}
