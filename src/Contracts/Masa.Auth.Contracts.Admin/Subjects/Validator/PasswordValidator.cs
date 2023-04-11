// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Configuration;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
using Microsoft.Extensions.Configuration;

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class PasswordValidator : MasaAbstractValidator<string?>
{
    public const string PASSWORDRULE_ConfigName = "AppSettings:PasswordRule";

    public PasswordValidator(IMasaConfiguration masaConfiguration)
    {
        var passwordRule = string.Empty;
        if(masaConfiguration != null)
        {
            masaConfiguration.ConfigurationApi.GetDefault().GetValue<string>(PASSWORDRULE_ConfigName, string.Empty);
        }

        if (string.IsNullOrWhiteSpace(passwordRule))
        {
            RuleFor(password => password).Required().Password();
        }
        else
        {
            RuleFor(password => password).Required().Password(passwordRule);
        }
    }
}
