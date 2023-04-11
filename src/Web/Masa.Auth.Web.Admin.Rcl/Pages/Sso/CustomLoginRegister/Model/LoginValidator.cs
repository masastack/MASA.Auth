// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Subjects.Validator;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator(PasswordValidator passwordValidator)
    {
        RuleFor(login => login.Account).Required().ChineseLetterNumber();
        RuleFor(user => user.Password).SetValidator(passwordValidator);
    }
}

