// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

internal class AddClientValidator : AbstractValidator<AddClientDto>
{
    public AddClientValidator()
    {
        RuleFor(client => client.ClientId).Required().WithMessage("ClientId is required");
        RuleFor(client => client.ClientName).Required().WithMessage("ClientName is required");
    }
}
