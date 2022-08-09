// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization.Position;

public partial class Position
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
            _page = 1;
            GetPositionsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetPositionsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetPositionsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public int DataIndex { get; set; } = 1;

    public List<PositionDto> Positions { get; set; } = new();

    public Guid CurrentPositionId { get; set; }

    public bool AddPositionDialogVisible { get; set; }

    public bool UpdatePositionDialogVisible { get; set; }

    private PositionService PositionService => AuthCaller.PositionService;

    protected override async Task OnInitializedAsync()
    {
        PageName = "PositionBlock";
        await GetPositionsAsync();
    }

    public List<DataTableHeader<PositionDto>> GetHeaders() => new()
    {
        new() { Text = T("Index"), Value = "Index", Sortable = false , Width="105px" },
        new() { Text = T("Name"), Value = nameof(PositionDto.Name), Sortable = false },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align="center", Width="105px" },
    };

    public async Task GetPositionsAsync()
    {
        Loading = true;
        var reuquest = new GetPositionsDto(Page, PageSize, Search);
        var response = await PositionService.GetListAsync(reuquest);
        Positions = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddApiResourceDialog()
    {
        AddPositionDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(PositionDto Position)
    {
        CurrentPositionId = Position.Id;
        UpdatePositionDialogVisible = true;
    }

    public async Task OpenRemovePositionDialog(PositionDto Position)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete position data"));
        if (confirm) await RemovePositionAsync(Position.Id);
    }

    public async Task RemovePositionAsync(Guid PositionId)
    {
        Loading = true;
        await PositionService.RemoveAsync(PositionId);
        OpenSuccessMessage(T("Delete position data success"));
        await GetPositionsAsync();
        Loading = false;
    }
}

