﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.DisplayName)
           .NotEmpty().WithMessage("NickName is required")
           .Required().ChineseLetterNumber().MaxLength(50, "NickName");
        RuleFor(user => user.Name).ChineseLetterNumber().MaxLength(50);
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.CompanyName).ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(user => user.Position).ChineseLetterNumber().MinLength(2).MaxLength(16);
        RuleFor(user => user.Account).Required()
                                     .Matches("^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9@.]+$")
                                     .WithMessage("Can only input chinese and letter and number and @ of {PropertyName}")
                                     .MinLength(8)
                                     .MaxLength(50);
        RuleFor(user => user.Department).ChineseLetterNumber().MinLength(2).MaxLength(16);
        RuleFor(user => user.Avatar).Url().Required();
    }
}

