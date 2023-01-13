﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Organization;

public class DepartmenCascader : SCascader<DepartmentDto, Guid>
{
    [Inject]
    public AuthCaller AuthCaller { get; set; } = default!;

    DepartmentService DepartmentService => AuthCaller.DepartmentService;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        ItemText = value => value.Name;
        ItemValue = value => value.Id;
        ItemChildren = value => value.Children;
        ChangeOnSelect = true;
        Class ??= "";
        Clearable = true;
        await base.SetParametersAsync(parameters);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Items = await DepartmentService.GetListAsync();        
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(Label))
            Label = I18n?.T("Department") ?? "";
        base.OnParametersSet();
    }
}
