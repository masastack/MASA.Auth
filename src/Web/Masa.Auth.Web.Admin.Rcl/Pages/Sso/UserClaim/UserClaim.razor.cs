// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.UserClaim;

public partial class UserClaim
{
    private string? _search;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetUserClaimsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetUserClaimsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetUserClaimsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<UserClaimDto> UserClaims { get; set; } = new();

    public int CurrentUserClaimId { get; set; }

    public bool AddUserClaimDialogVisible { get; set; }

    public bool UpdateUserClaimDialogVisible { get; set; }

    public List<DataTableHeader<UserClaimDto>> Headers { get; set; } = new();

    private UserClaimService UserClaimService => AuthCaller.UserClaimService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("UserClaim.Name"), Value = nameof(UserClaimDto.Name), Sortable = false },
            new() { Text = T(nameof(UserClaimDto.Description)), Value = nameof(UserClaimDto.Description), Sortable = false },
            new() { Text = T("Type"), Value = nameof(UserClaimDto.UserClaimType), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetUserClaimsAsync();
    }

    public async Task GetUserClaimsAsync()
    {
        Loading = true;
        var reuquest = new GetUserClaimsDto(Page, PageSize, Search);
        var response = await UserClaimService.GetListAsync(reuquest);
        UserClaims = response.Items;
        Total = response.Total;
        Loading = false;
    }

    private async Task AddStandardUserClaimsAsync()
    {
        Loading = true;
        await UserClaimService.AddStandardUserClaimsAsync();
        await GetUserClaimsAsync();
        Loading = false;
    }

    public void OpenAddApiResourceDialog()
    {
        AddUserClaimDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(UserClaimDto UserClaim)
    {
        CurrentUserClaimId = UserClaim.Id;
        UpdateUserClaimDialogVisible = true;
    }

    public async Task OpenRemoveUserClaimDialog(UserClaimDto UserClaim)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete userClaim data"));
        if (confirm) await RemoveUserClaimAsync(UserClaim.Id);
    }

    public async Task RemoveUserClaimAsync(int UserClaimId)
    {
        Loading = true;
        await UserClaimService.RemoveAsync(UserClaimId);
        OpenSuccessMessage(T("Delete userClaim data success"));
        await GetUserClaimsAsync();
        Loading = false;
    }
}

