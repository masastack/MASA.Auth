// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin.Model;

public class RegisterModel
{
    private Dictionary<string, bool> _requiredMap = new Dictionary<string, bool>();

    public string Account { get; set; } = "";

    public string Password { get; set; } = "";

    public string ConfirmPassword { get; set; } = "";

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public void Set(string filed, bool required)
    {
        _requiredMap[filed] = required;
    }

    public bool CheckRequired(string filed)
    {
        if (_requiredMap.ContainsKey(filed)) return _requiredMap[filed];
        else return false;
    }

    public static Dictionary<RegisterFieldTypes, RegisterFieldValueTypes> FiledMap = new Dictionary<RegisterFieldTypes, RegisterFieldValueTypes>
    {
        [RegisterFieldTypes.Account] = RegisterFieldValueTypes.String,
        [RegisterFieldTypes.Password] = RegisterFieldValueTypes.Password,
    };
}

