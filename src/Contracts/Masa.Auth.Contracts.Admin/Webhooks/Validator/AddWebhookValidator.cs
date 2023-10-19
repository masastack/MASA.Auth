// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Webhooks.Validator;

internal class AddWebhookValidator : AbstractValidator<WebhookDetailDto>
{
    public AddWebhookValidator()
    {
        RuleFor(p => p.Name).Required().ChineseLetterNumber().MinimumLength(2).MaximumLength(20);
        RuleFor(p => p.HttpMethod).Required();
        RuleFor(p => p.Url).Required().Url();
        RuleFor(p => p.Event).Required();
    }
}
