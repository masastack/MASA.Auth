// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UserSystemDataDtoValidator : AbstractValidator<UserSystemDataDto>
{
    public UserSystemDataDtoValidator()
    {
        RuleFor(user => user.UserId).NotEmpty();
        RuleFor(user => user.SystemId).NotEmpty();
    }
}

