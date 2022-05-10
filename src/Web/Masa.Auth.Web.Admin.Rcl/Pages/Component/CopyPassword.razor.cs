// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class CopyPassword
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Value { get; set; } = "";

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    public async Task ResetPassword()
    {
        Random random = new Random();
        Value = random.Next(111111, 999999).ToString();
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }
}

