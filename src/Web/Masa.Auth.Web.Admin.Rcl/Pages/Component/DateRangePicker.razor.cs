// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class DateRangePicker
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public DateOnly? StartTime { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> StartTimeChanged { get; set; }

    [Parameter]
    public DateOnly? EndTime { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> EndTimeChanged { get; set; }

    private bool StartTimeVisible { get; set; }

    private bool EndTimeVisible { get; set; }

    private async Task UpdateStartTimeAsync(DateOnly? dateTime)
    {
        if (dateTime > EndTime) OpenWarningMessage(T("Start time cannot be greater than end time"));
        else
        {
            StartTime = dateTime;
            if (StartTimeChanged.HasDelegate) await StartTimeChanged.InvokeAsync(dateTime);
        }
    }

    private async Task UpdateEndTimeAsync(DateOnly? dateTime)
    {
        if (dateTime < StartTime) OpenWarningMessage(T("End time cannot be less than start time"));
        else
        {
            EndTime = dateTime;
            if (StartTimeChanged.HasDelegate) await EndTimeChanged.InvokeAsync(dateTime);
        }
    }
}

