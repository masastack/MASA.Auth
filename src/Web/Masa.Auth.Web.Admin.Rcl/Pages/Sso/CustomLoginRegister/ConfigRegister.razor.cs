// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister;

public partial class ConfigRegister
{
    [Parameter]
    public List<RegisterFieldDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<RegisterFieldDto>> ValueChanged { get; set; }

    [Parameter]
    public string? Logo { get; set; }

    [Parameter]
    public string? Title { get; set; }

    protected override void OnInitialized()
    {
        if (Value.Count == 0)
        {
            Value.Add(new RegisterFieldDto(RegisterFieldTypes.Account, 1, true, true));
            Value.Add(new RegisterFieldDto(RegisterFieldTypes.Password, 2, true, true));
        }
    }

    public async Task Up(RegisterFieldDto registerField)
    {
        registerField.Sort--;
        Value.Remove(registerField);
        Value.Insert(registerField.Sort - 1, registerField);
        InitSort();
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task Down(RegisterFieldDto registerField)
    {
        registerField.Sort++;
        Value.Remove(registerField);
        Value.Insert(registerField.Sort - 1, registerField);
        InitSort();
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task Remove(RegisterFieldDto registerField)
    {
        Value.Remove(registerField);
        InitSort();
        await ValueChanged.InvokeAsync(Value);
    }

    public void InitSort()
    {
        int sort = 0;
        foreach (var registerField in Value)
        {
            registerField.Sort = ++sort;
        }
        Value = Value.OrderBy(v => v.Sort).ToList();
    }
}

