// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization.Position;

public partial class Position
{
    private string? _search;
    private int _page = 1;
    private int _oldPage = 1, _pageSize = 20;

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
            GetPositionsAsync().ContinueWith(_ =>
            {
                _oldPage = value;
                InvokeAsync(StateHasChanged);
            });
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
    
    private List<DataTableHeader<PositionDto>> _headers = new();

    protected override async Task OnInitializedAsync()
    {
        PageName = "PositionBlock";

        _headers = new List<DataTableHeader<PositionDto>>() {
            new() { Text = T("Index"), Value = "Index", Sortable = false, Width = "105px" },
            new() { Text = T("Name"), Value = nameof(PositionDto.Name), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width = "105px" },
        };
        
        await GetPositionsAsync();
    }

    public async Task GetPositionsAsync()
    {
        var reuquest = new GetPositionsDto(Page, PageSize, Search);
        var response = await PositionService.GetListAsync(reuquest);
        Positions = response.Items;
        Total = response.Total;
    }

    public void OpenAddApiResourceDialog()
    {
        AddPositionDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(PositionDto position)
    {
        CurrentPositionId = position.Id;
        UpdatePositionDialogVisible = true;
    }

    public async Task OpenRemovePositionDialog(PositionDto position)
    {
        var confirm = await OpenConfirmDialog(T("Delete Position"), T("Are you sure delete position \"{0}\"?", position.Name));
        if (confirm) await RemovePositionAsync(position.Id);
    }

    public async Task RemovePositionAsync(Guid positionId)
    {
        await PositionService.RemoveAsync(positionId);
        OpenSuccessMessage(T("Delete position data success"));
        await GetPositionsAsync();
    }
}

