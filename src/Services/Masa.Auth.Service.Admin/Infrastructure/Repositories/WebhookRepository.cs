// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class WebhookRepository : Repository<AuthDbContext, Webhook, Guid>, IWebhookRepository
{
    public WebhookRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
