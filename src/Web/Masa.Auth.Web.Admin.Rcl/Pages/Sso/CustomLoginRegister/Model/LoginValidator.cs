// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(login => login.Account).Required().ChineseLetterNumber();
        RuleFor(user => user.Password).Required()
                                      .Matches(@"^\S*(?=\S{8,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
                                      .WithMessage("Password must contain numbers and letter, and not less than 8 digits")
                                      .MaxLength(30);
    }
}

