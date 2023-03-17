// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : MasaAbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.DisplayName)
            .Required().WithMessage("NickNameBlock.Required")
            .ChineseLetterNumber().WithMessage("NickNameBlock.ChineseLetterNumber")
            .MaximumLength(50).WithMessage("NickNameBlock.MaxLength")
            .WithName("NickName"); ;
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Account).Required()
                                     .Matches("^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9@.]+$")
                                     .WithMessage("Can only input chinese and letter and number and @ of {PropertyName}")
                                     .MinimumLength(8)
                                     .MaximumLength(50);
        RuleFor(user => user.Avatar).Url().Required();
        WhenNotEmpty(user => user.Email, r => r.Email());
        WhenNotEmpty(user => user.Name, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        WhenNotEmpty(user => user.IdCard, r => r.IdCard());
        WhenNotEmpty(user => user.CompanyName, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        WhenNotEmpty(user => user.Position, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(16));
        WhenNotEmpty(user => user.Department, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(16));
    }
}
