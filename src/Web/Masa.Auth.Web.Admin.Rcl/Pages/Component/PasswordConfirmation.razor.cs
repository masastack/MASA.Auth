// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class PasswordConfirmation
{
    public bool Visible { get; set; }

    public bool Value { get; set; }

    public EventCallback<bool> ValueChanged { get; set; }

    public string? ConfirmText { get; set; }

    public async Task CancelAsync() 
    {
        Visible = false;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(false);
        else Value = false;
    }

    public async Task OkAsync()
    {
        Visible = false;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(true);
        else Value = true;
    }
}
