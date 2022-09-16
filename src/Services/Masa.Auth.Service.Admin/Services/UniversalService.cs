// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class UniversalService : ServiceBase
{
    public UniversalService(IServiceCollection services) : base(services, "api/universal")
    {
        App.MapPost("send_sms", SendSMSAsync);
        App.MapPost("send_email", SendEmailAsync);
    }

    private async Task SendSMSAsync(IEventBus eventBus,
        [FromBody] SendMsgCodeModel model)
    {
        await eventBus.PublishAsync(new SendSMSCommand(model));
    }

    private async Task SendEmailAsync(IEventBus eventBus,
        [FromBody] SendEmailModel sendEmailModel)
    {
        await eventBus.PublishAsync(new SendEmailCommand(sendEmailModel));
    }
}
