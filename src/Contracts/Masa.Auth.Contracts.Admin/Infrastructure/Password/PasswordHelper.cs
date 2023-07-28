// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Password;

public class PasswordHelper : ISingletonDependency
{
    public const string PASSWORDRULECONFIGNAME = "$public.AppSettings:PasswordRule";
    public const string DEFAULTPASSWORDRULE = "^\\S*(?=\\S{6,})(?=\\S*\\d)(?=\\S*[A-Za-z])\\S*$";

    private IMasaConfiguration _masaConfiguration;

    public PasswordHelper(IMasaConfiguration masaConfiguration)
    {
        _masaConfiguration = masaConfiguration;
    }

    public string GenerateNewPassword()
    {
        var passwordRule = GetPasswordRule();
        var xeger = new Xeger(passwordRule);
        return xeger.Generate();
    }

    public string GetPasswordRule()
    {
        var passwordRule = DEFAULTPASSWORDRULE;
        if (_masaConfiguration != null)
        {
            try
            {
                passwordRule = _masaConfiguration.ConfigurationApi.GetPublic().GetValue(PASSWORDRULECONFIGNAME, DEFAULTPASSWORDRULE);
            }
            catch
            {
                //ignore
            }
            finally
            {
                passwordRule = DEFAULTPASSWORDRULE;
            }

        }
        return passwordRule;
    }

    public void ValidatePassword(string? password, ValidationContext<string?> context)
    {
        if (password == null)
        {
            password = string.Empty;
        }

        var passwordRule = GetPasswordRule();
        if (!Regex.IsMatch(password, passwordRule))
        {
            var message = DEFAULTPASSWORDRULE.Equals(passwordRule) ? "PasswordFormatVerificationPrompt" : "PasswordValidateFailed";
            try
            {
                message = I18n.T(message);
            }
            catch (Exception) { }
            finally
            {
                context.AddFailure(message);
            }
        }
    }
}