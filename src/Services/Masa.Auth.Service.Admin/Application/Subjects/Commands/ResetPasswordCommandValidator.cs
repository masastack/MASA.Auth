// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        When(x => x.ResetPasswordType == ResetPasswordTypes.PhoneNumber, () =>
        {
            RuleFor(x => x.Voucher).NotEmpty().Phone(CultureInfo.CurrentUICulture.Name);
        });
        When(x => x.ResetPasswordType == ResetPasswordTypes.Email, () =>
        {
            RuleFor(x => x.Voucher).NotEmpty().Email();
        });
        RuleFor(x => x.Captcha).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
    }
}
