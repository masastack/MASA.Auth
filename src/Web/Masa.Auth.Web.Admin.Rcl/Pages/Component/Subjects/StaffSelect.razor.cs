// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class StaffSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public List<Guid>? IgnoreValue { get; set; }

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public RoleLimitModel RoleLimit { get; set; } = new("", int.MaxValue);

    private bool _staffLoading;
    private List<Guid> _ignoreValue = new();
    private CancellationTokenSource? _cts;

    protected List<StaffSelectDto> Staffs { get; set; } = new();

    protected StaffService StaffService => AuthCaller.StaffService;

    protected override void OnInitialized()
    {
        Label = T("Staff");
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Staffs = await StaffService.SelectByIdsAsync(Value);
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        if (IgnoreValue?.SequenceEqual(_ignoreValue) == false)
        {
            _ignoreValue = IgnoreValue;
            Staffs.RemoveAll(s => IgnoreValue.Contains(s.Id));
        }
        base.OnParametersSet();
    }

    protected async Task RemoveStaff(StaffSelectDto staff)
    {
        if (Readonly is false)
        {
            var value = new List<Guid>();
            value.AddRange(Value);
            value.Remove(staff.Id);
            await UpdateValueAsync(value);
        }
    }

    public async Task UpdateValueAsync(List<Guid> value)
    {
        if (value.Count > RoleLimit.Limit)
        {
            value.Remove(value.Last());
            OpenErrorMessage(string.Format(T("Due to the role [{0}] limit constraint, a maximum of {1} members can be selected"), RoleLimit.Role, RoleLimit.Limit));
        }
        else
        {
            if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
            else Value = value;
        }
    }

    private bool FilterItem(StaffSelectDto item, string queryText, string itemText)
    {
        return item.DisplayName.Contains(queryText, StringComparison.OrdinalIgnoreCase);
    }

    private async Task QuerySelectionStaff(string search)
    {
        _cts?.Cancel();
        _cts = new();
        try
        {
            await Task.Delay(300, _cts.Token);

            if (string.IsNullOrWhiteSpace(search))
            {
                return;
            }

            search = search.Trim();
            _staffLoading = true;
            StateHasChanged();
            
            var staffs = await StaffService.GetSelectAsync(search);
            var selectedItems = Staffs.Where(s => Value.Contains(s.Id)).ToList();
            Staffs = selectedItems.UnionBy(staffs, staff => staff.Id).ToList();
            Staffs.RemoveAll(s => IgnoreValue?.Contains(s.Id) == true);
            _staffLoading = false;
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }
}

