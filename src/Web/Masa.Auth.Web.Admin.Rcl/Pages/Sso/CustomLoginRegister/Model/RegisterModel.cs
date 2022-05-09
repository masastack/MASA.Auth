// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class RegisterModel
{    
    public string Account { get; set; } = "";

    public string Password { get; set; } = "";

    public string ConfirmPassword { get; set; } = "";

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public List<string> RequiredFileds = new ();

    public bool CheckRequired(string filed) => RequiredFileds.Contains(filed);
}

