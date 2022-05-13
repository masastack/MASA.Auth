// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class ThirdPartyIdp
{
    private string? _search;
    private bool _enabled;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<ThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public Guid CurrentThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyIdpDto>> Headers { get; set; } = new();

    public bool AddThirdPartyIdpDialogVisible { get; set; }

    public bool UpdateThirdPartyIdpDialogVisible { get; set; }

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyIdpDto.Icon), Sortable = false },
            new() { Text = T("ThirdPartyIdp.Name"), Value = nameof(ThirdPartyIdpDto.Name), Sortable = false },
            new() { Text = T("ThirdPartyIdp.DisplayName"), Value = nameof(ThirdPartyIdpDto.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyIdpDto.VerifyType), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpDto.CreationTime)), Value = nameof(ThirdPartyIdpDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpDto.Url)), Value = nameof(ThirdPartyIdpDto.Url), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetThirdPartyIdpsAsync();
    }

    public async Task GetThirdPartyIdpsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyIdpsDto(Page, PageSize, Search);
        var response = await ThirdPartyIdpService.GetListAsync(request);
        ThirdPartyIdps = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddThirdPartyIdpDialog()
    {
        AddThirdPartyIdpDialogVisible = true;
    }

    public void OpenUpdateThirdPartyIdpDialog(ThirdPartyIdpDto thirdPartyIdp)
    {
        CurrentThirdPartyIdpId = thirdPartyIdp.Id;
        UpdateThirdPartyIdpDialogVisible = true;
    }

    public async Task OpenRemoveThirdPartyIdpDialog(ThirdPartyIdpDto thirdPartyIdp)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete data"));
        if (confirm) await RemoveThirdPartyIdpAsync();
    }

    public async Task RemoveThirdPartyIdpAsync()
    {
        Loading = true;
        await ThirdPartyIdpService.RemoveAsync(CurrentThirdPartyIdpId);
        OpenSuccessMessage(T("Success to delete thirdPartyIdp"));
        Loading = false;
    }
}

