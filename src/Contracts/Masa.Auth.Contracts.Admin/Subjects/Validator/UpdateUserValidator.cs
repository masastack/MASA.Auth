// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.DisplayName).Required().MaxLength(50);
        RuleFor(user => user.Name).ChineseLetter().MaxLength(20);
        RuleFor(user => user.PhoneNumber).Required().Phone();      
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.Position).ChineseLetterNumber().MaxLength(20);
    }
}

