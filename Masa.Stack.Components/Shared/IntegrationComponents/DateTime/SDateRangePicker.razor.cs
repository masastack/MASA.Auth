// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SDateRangePicker
{
    [Parameter]
    public DateOnly? StartTime { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> StartTimeChanged { get; set; }

    [Parameter]
    public DateOnly? EndTime { get; set; }

    [Parameter]
    public EventCallback<DateOnly?> EndTimeChanged { get; set; }

    [Parameter]
    public EventCallback<(DateOnly?, DateOnly?)> DateRangeChanged { get; set; }

    [Parameter]
    public StringBoolean? Attach { get; set; }

    private bool StartTimeVisible { get; set; }

    private bool EndTimeVisible { get; set; }

    private string _datetimeStartTextCss = "body2 regular3--text";

    private string _datetimeEndTextCss = "body2 regular3--text";

    private async Task UpdateStartTimeAsync(DateOnly? dateTime)
    {
        if (dateTime > EndTime) await PopupService.EnqueueSnackbarAsync(T("Start time cannot be greater than end time"), AlertTypes.Warning);
        else
        {
            StartTime = dateTime;
            _datetimeStartTextCss = $"body2 {(dateTime is null ? "regular3--text" : "regular--text")}";

            if (StartTimeChanged.HasDelegate) await StartTimeChanged.InvokeAsync(dateTime);
            if (DateRangeChanged.HasDelegate) await DateRangeChanged.InvokeAsync((StartTime, EndTime));
        }
    }

    private async Task UpdateEndTimeAsync(DateOnly? dateTime)
    {
        if (dateTime < StartTime) await PopupService.EnqueueSnackbarAsync(T("End time cannot be less than start time"), AlertTypes.Warning);
        else
        {
            EndTime = dateTime;
            _datetimeEndTextCss = $"body2 {(dateTime is null ? "regular3--text" : "regular--text")}";

            if (StartTimeChanged.HasDelegate) await EndTimeChanged.InvokeAsync(dateTime);
            if (DateRangeChanged.HasDelegate) await DateRangeChanged.InvokeAsync((StartTime, EndTime));
        }
    }
}

