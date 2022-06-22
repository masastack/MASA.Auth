// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.DisplayName).ChineseLetter().MinLength(1).MaxLength(20);
        RuleFor(user => user.Name).ChineseLetter().MaxLength(20);
        RuleFor(user => user.PhoneNumber).Phone().When(u => !string.IsNullOrEmpty(u.PhoneNumber));
        //RuleFor(user => user.Landline).NotEmpty().When(u => string.IsNullOrEmpty(u.PhoneNumber));
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.Position).ChineseLetterNumber().MaxLength(20);
        RuleFor(user => user.Password).Required()
                              .Matches(@"^\S*(?=\S{8,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
                              .WithMessage("Password must contain numbers and letter, and not less than 8 digits")
                              .MaxLength(30);
    }
}

