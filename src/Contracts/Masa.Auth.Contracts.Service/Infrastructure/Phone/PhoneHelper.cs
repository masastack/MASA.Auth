// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.
namespace Masa.Auth.Contracts.Admin.Infrastructure.Phone;

public class PhoneHelper : ISingletonDependency
{
    public const string PHONERULECONFIGNAME = "$public.AppSettings:PhoneRule";

    private readonly IMasaConfiguration _masaConfiguration;

    public PhoneHelper(IMasaConfiguration masaConfiguration)
    {
        _masaConfiguration = masaConfiguration;
    }

    private string GetPhoneRule()
    {
        var rule = string.Empty;
        try
        {
            var key = $"{PHONERULECONFIGNAME}:{GlobalValidationOptions.DefaultCulture}";
            rule = _masaConfiguration.ConfigurationApi.GetPublic().GetValue(key, string.Empty);
        }
        catch
        {
            //ignore
        }
        return rule;
    }

    public void ValidatePhone(string? phone, ValidationContext<string?> context)
    {
        if (phone == null)
        {
            phone = string.Empty;
        }

        var isValid = false;
        var phoneRule = GetPhoneRule();
        if (string.IsNullOrEmpty(phoneRule))
        {
            var phoneValidator = new PhoneValidator<string?>(GlobalValidationOptions.DefaultCulture);
            isValid = phoneValidator.IsValid(context, phone);
        }
        else
        {
            isValid = Regex.IsMatch(phone, phoneRule);
        }

        if (!isValid)
        {
            var message = "PhoneValidateFailed";
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
