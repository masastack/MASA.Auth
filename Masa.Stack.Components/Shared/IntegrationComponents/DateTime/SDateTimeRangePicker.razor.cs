// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SDateTimeRangePicker
{
    [Parameter, EditorRequired]
    public DateTimeOffset? StartDateTime { get; set; }

    [Parameter]
    public StringNumber? MaxPickerWidth { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset?> StartDateTimeChanged { get; set; }

    [Parameter, EditorRequired]
    public DateTimeOffset? EndDateTime { get; set; }

    [Parameter]
    public EventCallback<DateTimeOffset?> EndDateTimeChanged { get; set; }

    [Parameter]
    public EventCallback OnConfirm { get; set; }

    [Parameter]
    public EventCallback OnClear { get; set; }

    [Parameter]
    public EventCallback<TimeZoneInfo> OnTimeZoneInfoChange { get; set; }

    [Parameter]
    public Func<DateOnly?, DateOnly?, bool>? StartTimeLimit { get; set; }

    [Parameter]
    public Func<DateOnly?, DateOnly?, bool>? EndTimeLimit { get; set; }

    [Parameter]
    public bool ShowTimeZoneSelector { get; set; } = false;

    [Parameter]
    public bool Clearable { get; set; } = false;

    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    private static ReadOnlyCollection<TimeZoneInfo> _systemTimeZones = TimeZoneInfo.GetSystemTimeZones();

    private TimeSpan _internalOffset;

    private bool _menuValue;

    private TimeSpan _offset;

    private DateTimeOffset? _internalStartDateTime;
    private DateTimeOffset? _internalEndDateTime;

    private DateOnly? _internalStartDate;
    private DateOnly? _internalEndDate;
    private TimeOnly _internalStartTime;
    private TimeOnly _internalEndTime;

    private TimeZoneInfo? _timeZone;

    private DateTimeOffset? _lastStartDateTime;

    private DateTimeOffset? _lastEndDateTime;

    private static readonly TimeOnly DefaultTimeOnly = new(0, 0, 0);

    private bool HasTimeChange => _lastStartDateTime != _internalStartDateTime || _lastEndDateTime != _internalEndDateTime;


    private bool HasTimeZoneChange => _offset != _internalOffset;

    private string DateTimeButtonStyle => $"font-size: 14px; width: calc(100% - {(ShowTimeZoneSelector ? 66 : 24) - (Clearable ? 0 : 24)}px  )";


    private string MinWidth => ShowTimeZoneSelector ? "447px" : "405px";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            UpdateInternalStartDateTime(StartDateTime);
            UpdateInternalEndDateTime(EndDateTime);

            _offset = JsInitVariables.TimezoneOffset;
            _internalOffset = _offset;
            _timeZone = GetSelectTimeZone();
            StateHasChanged();
        }
    }

    private void MenuValueChanged(bool val)
    {
        _menuValue = val;

        if (!val) return;

        UpdateInternalStartDateTime(StartDateTime);
        UpdateInternalEndDateTime(EndDateTime);
        _internalOffset = _offset;
    }

    private void UpdateTimeZone()
    {
        if (_internalEndDateTime != null)
            _internalEndDateTime = new DateTimeOffset(ticks: _internalEndDateTime.Value.Ticks, offset: _internalOffset);
        if (_internalStartDateTime.HasValue)
            _internalStartDateTime = new DateTimeOffset(ticks: _internalStartDateTime.Value.Ticks, offset: _internalOffset);
    }

    private void UpdateInternalStartDateTime(DateTimeOffset? val)
    {
        _lastStartDateTime = _internalStartDateTime;
        _internalStartDateTime = val;
        if (val.HasValue)
        {
            _internalStartDate = DateOnly.FromDateTime(val.Value.DateTime);
            _internalStartTime = TimeOnly.FromDateTime(val.Value.DateTime);
        }
        else
        {
            _internalStartTime = DefaultTimeOnly;
            _internalStartDate = null;
        }
    }

    private void UpdateInternalEndDateTime(DateTimeOffset? val)
    {
        _lastEndDateTime = _internalEndDateTime;
        _internalEndDateTime = val;
        if (val.HasValue)
        {
            _internalEndDate = DateOnly.FromDateTime(val.Value.DateTime);
            _internalEndTime = TimeOnly.FromDateTime(val.Value.DateTime);
        }
        else
        {
            _internalStartTime = DefaultTimeOnly;
            _internalEndDate = null;
        }
    }

    private void InternalStartDateChanged(DateOnly? val)
    {
        _lastStartDateTime = _internalStartDateTime;
        _internalStartDate = val;
        if (val != null)
        {
            _internalStartDateTime = new DateTimeOffset(val.Value.Year, val.Value.Month, val.Value.Day,
                _internalStartDateTime?.Hour ?? 0,
                _internalStartDateTime?.Minute ?? 0,
                _internalStartDateTime?.Second ?? 0,
                _internalOffset);
        }
        else
        {
            _internalStartDateTime = null;
        }
    }

    private void InternalEndDateChanged(DateOnly? val)
    {
        _lastEndDateTime = _internalEndDateTime;
        _internalEndDate = val;
        if (val != null)
        {
            _internalEndDateTime = new DateTimeOffset(val.Value.Year, val.Value.Month, val.Value.Day,
                _internalEndTime.Hour,
                _internalEndTime.Minute,
                _internalEndTime.Second,
                _internalOffset);
        }
        else
        {
            _internalEndDateTime = null;
        }
    }

    private void InternalStartTimeChanged(TimeOnly val)
    {
        _lastStartDateTime = _internalStartDateTime;
        _internalStartTime = val;
        if (_internalStartDateTime != null)
        {
            _internalStartDateTime = new DateTimeOffset(
                _internalStartDateTime.Value.Date.Year,
                _internalStartDateTime.Value.Date.Month,
                _internalStartDateTime.Value.Date.Day,
                val.Hour,
                val.Minute,
                val.Second,
                _internalOffset
            );
        }
    }

    private void InternalEndTimeChanged(TimeOnly val)
    {
        _lastEndDateTime = _internalEndDateTime;
        _internalEndTime = val;
        if (_internalEndDateTime != null)
        {
            _internalEndDateTime = new DateTimeOffset(
                _internalEndDateTime.Value.Date.Year,
                _internalEndDateTime.Value.Date.Month,
                _internalEndDateTime.Value.Date.Day,
                val.Hour,
                val.Minute,
                val.Second,
                _internalOffset
            );
        }
    }

    private async Task HandleOnConfirm()
    {
        if (StartDateTimeChanged.HasDelegate)
        {
            await StartDateTimeChanged.InvokeAsync(_internalStartDateTime);
        }
        else
        {
            StartDateTime = _internalStartDateTime;
        }

        if (EndDateTimeChanged.HasDelegate)
        {
            await EndDateTimeChanged.InvokeAsync(_internalEndDateTime);
        }
        else
        {
            EndDateTime = _internalEndDateTime;
        }

        if (HasTimeZoneChange && OnTimeZoneInfoChange.HasDelegate)
        {
            _offset = _internalOffset;
            _timeZone = GetSelectTimeZone();
            await OnTimeZoneInfoChange.InvokeAsync(_timeZone);
        }

        if (HasTimeChange && OnConfirm.HasDelegate)
        {
            _ = OnConfirm.InvokeAsync();
        }
        _menuValue = false;
    }

    private void OnInternalOffsetUpdated(TimeZoneInfo timeZoneInfo)
    {
        _internalOffset = timeZoneInfo.BaseUtcOffset;
        if (_internalStartDateTime.HasValue)
            UpdateInternalStartDateTime(_internalStartDateTime.Value.ToOffset(_internalOffset));
        if (_internalEndDateTime.HasValue)
            UpdateInternalEndDateTime(_internalEndDateTime.Value.ToOffset(_internalOffset));
        UpdateTimeZone();
    }

    private void CancelClick()
    {
        if (HasTimeZoneChange)
        {
            if (OnTimeZoneInfoChange.HasDelegate)
                _ = OnTimeZoneInfoChange.InvokeAsync(_timeZone);
            OnInternalOffsetUpdated(_timeZone!);
        }
        _menuValue = false;
    }

    private async Task ClearClick()
    {
        UpdateInternalStartDateTime(null);
        UpdateInternalEndDateTime(null);
        StartDateTime = null;
        EndDateTime = null;
        if (StartDateTimeChanged.HasDelegate)
        {
            await StartDateTimeChanged.InvokeAsync(_internalStartDateTime);
        }
        else
        {
            StartDateTime = _internalStartDateTime;
        }

        if (EndDateTimeChanged.HasDelegate)
        {
            await EndDateTimeChanged.InvokeAsync(_internalEndDateTime);
        }
        else
        {
            EndDateTime = _internalEndDateTime;
        }

        if (OnClear.HasDelegate)
        {
            _ = OnClear.InvokeAsync();
        }
    }

    private string FormatDateTime(DateTimeOffset? dateTime, string placeholder = "")
    {
        var result = dateTime?.ToString(I18n.T("$DateTimeFormat"));
        if (string.IsNullOrEmpty(result))
        {
            return placeholder;
        }
        return result;
    }

    private TimeZoneInfo GetSelectTimeZone()
    {
        return _systemTimeZones.FirstOrDefault(timeZone => timeZone.BaseUtcOffset == _internalOffset)!;
    }

    private string GetSelectTimeString()
    {
        var start = FormatDateTime(_internalStartDateTime);

        var end = FormatDateTime(_internalEndDateTime);
        if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
        {
            return "";
        }

        if (string.IsNullOrEmpty(start))
        {
            return T(scope: DateTimeRangeScope, "Until", end);
        }

        if (string.IsNullOrEmpty(end))
        {
            return T(scope: DateTimeRangeScope, "From", start);
        }

        return T(scope: DateTimeRangeScope, "FromTo", start, end);
    }
}
