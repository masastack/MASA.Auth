using Masa.BuildingBlocks.Configuration;
using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
using Microsoft.Extensions.Configuration;

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class PasswordValidator : MasaAbstractValidator<string?>
{
    public const string PASSWORDRULE_ConfigName = "$public.AppSettings:PasswordRule";

    public PasswordValidator(IMasaConfiguration masaConfiguration)
    {
        var passwordRule = string.Empty;
        if(masaConfiguration != null)
        {
            passwordRule = masaConfiguration.ConfigurationApi.GetPublic().GetValue<string>(PASSWORDRULE_ConfigName, string.Empty);
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
