// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SDateTimeTitle
{
    DateTime? _value;

    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    [Parameter]
    public bool DateTime { get; set; }

    [Parameter]
    public bool Date { get; set; }

    [Parameter]
    public bool Time { get; set; }

    [Parameter]
    public TimeSpan DisplayTimezoneOffset { get; set; }

    [Parameter]
    public DateTime? Value
    {
        get => _value;
        set
        {
            if (value != null && value != new DateTime())
            {
                _value = value?.Add(DisplayTimezoneOffset);
            }
            else
            {
                _value = value;
            }
        }
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        DisplayTimezoneOffset = JsInitVariables.TimezoneOffset;
        return base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        if ((DateTime, Date, Time) == (false, false, false))
        {
            Date = true;
            Time = true;
        }
    }
}
