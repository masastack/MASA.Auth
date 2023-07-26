// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.UserClaim;

public partial class UserClaim
{
    private string? _search;
    private int _page = 1, _pageSize = 20;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
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
            _page = 1;
            _pageSize = value;
            GetUserClaimsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<UserClaimDto> UserClaims { get; set; } = new();

    public Guid CurrentUserClaimId { get; set; }

    public bool AddUserClaimDialogVisible { get; set; }

    public bool UpdateUserClaimDialogVisible { get; set; }

    private UserClaimService UserClaimService => AuthCaller.UserClaimService;

    protected override async Task OnInitializedAsync()
    {
        PageName = "UserClaimBlock";
        await GetUserClaimsAsync();
    }

    public List<DataTableHeader<UserClaimDto>> GetHeaders() => new()
    {
        new() { Text = T("Name"), Value = nameof(UserClaimDto.Name), Sortable = false, Width="250px" },
        new() { Text = T(nameof(UserClaimDto.Description)), Value = nameof(UserClaimDto.Description), Sortable = false },
        new() { Text = T("Type"), Value = nameof(UserClaimDto.UserClaimType), Sortable = false, Width="105px" },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetUserClaimsAsync()
    {
        var reuquest = new GetUserClaimsDto(Page, PageSize, Search);
        var response = await UserClaimService.GetListAsync(reuquest);
        UserClaims = response.Items;
        Total = response.Total;
    }

    private async Task AddStandardUserClaimsAsync()
    {
        var confirm = await OpenConfirmDialog(T("Operation confirmation"), T("Are you sure create standard userClaims"), AlertTypes.Info);
        if (confirm)
        {
            await UserClaimService.AddStandardUserClaimsAsync();
            OpenSuccessMessage(T("Add standard userClaim data success"));
            await GetUserClaimsAsync();
        }
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
        var confirm = await OpenConfirmDialog(T("Delete UserClaim"), T("Are you sure delete userClaim \"{0}\"?", UserClaim.Name));
        if (confirm) await RemoveUserClaimAsync(UserClaim.Id);
    }

    public async Task RemoveUserClaimAsync(Guid UserClaimId)
    {
        await UserClaimService.RemoveAsync(UserClaimId);
        OpenSuccessMessage(T("Delete userClaim data success"));
        await GetUserClaimsAsync();
    }
}

