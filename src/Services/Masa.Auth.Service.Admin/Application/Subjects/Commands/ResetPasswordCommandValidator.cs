// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(PhoneNumberValidator phoneValidator)
    {
        When(x => x.ResetPasswordType == ResetPasswordTypes.PhoneNumber, () =>
        {
            RuleFor(x => x.Voucher).Required().SetValidator(phoneValidator);
        });
        When(x => x.ResetPasswordType == ResetPasswordTypes.Email, () =>
        {
            RuleFor(x => x.Voucher).Required().Email();
        });
        RuleFor(x => x.Captcha).Required();
        RuleFor(x => x.Password).Required();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }
}
