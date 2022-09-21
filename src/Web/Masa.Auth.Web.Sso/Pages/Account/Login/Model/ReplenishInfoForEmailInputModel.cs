// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class ReplenishInfoForEmailInputModel : ReplenishInfoInputModel
{
    public string PhoneNumber { get; set; } = string.Empty;

    public int SmsCode { get; set; }
}
