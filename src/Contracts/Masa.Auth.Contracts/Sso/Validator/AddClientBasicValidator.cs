// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddClientBasicValidator : MasaAbstractValidator<AddClientBasicDto>
{
    public AddClientBasicValidator()
    {
        RuleFor(client => client.ClientId).Required().Identity();
        RuleFor(client => client.ClientName).Required().MaximumLength(50);
        RuleFor(client => client.Description).MaximumLength(200);
        WhenNotEmpty(c => c.ClientUri, r => r.Url());
    }
}
