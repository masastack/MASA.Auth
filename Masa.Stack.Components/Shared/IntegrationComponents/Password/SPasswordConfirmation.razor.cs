// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SPasswordConfirmation
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnOk { get; set; }

    [Parameter]
    public EventCallback Cancel { get; set; }

    [Parameter]
    public string ConfirmText { get; set; } = "";

    string WriteText { get; set; } = "";

    List<string> ErrorMessages = new();

    public async Task CancelAsync()
    {
        if (Cancel.HasDelegate) await Cancel.InvokeAsync();
        if (VisibleChanged.HasDelegate) await VisibleChanged.InvokeAsync(false);
        else Visible = false;
        ErrorMessages.Clear();
        WriteText = "";
    }

    public async Task OkAsync()
    {
        var confirm = ConfirmText == WriteText;
        if (confirm is false)
        {
            ErrorMessages = new() { T("Please enter as prompted.") };
        }
        else if (confirm is true && OnOk.HasDelegate)
        {
            await OnOk.InvokeAsync();
            if (VisibleChanged.HasDelegate) await VisibleChanged.InvokeAsync(false);
            else Visible = false;
            WriteText = "";
        }
    }
}
