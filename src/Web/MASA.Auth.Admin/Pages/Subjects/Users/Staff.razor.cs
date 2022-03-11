namespace Masa.Auth.Admin.Pages.Subjects.Users;

public partial class Staff
{
    private string? _search;
    private bool _enabled;
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<StaffItemResponse> Staffs { get; set; } = new();

    public Guid CurrentStaffId { get; set; }

    public List<DataTableHeader<StaffItemResponse>> Headers { get; set; } = new();

    public bool StaffDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserItemResponse.Avatar)), Value = nameof(UserItemResponse.Avatar), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.Name)), Value = nameof(UserItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Department"), Value = nameof(StaffItemResponse.DepartmentId), Sortable = false },
            new() { Text = T(nameof(StaffItemResponse.JobNumber)), Value = nameof(StaffItemResponse.JobNumber), Sortable = false },
            new() { Text = T(nameof(StaffItemResponse.Position)), Value = nameof(StaffItemResponse.Position), Sortable = false },
            new() { Text = T("State"), Value = nameof(StaffItemResponse.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetStaffAsync();
    }

    public async Task GetStaffAsync()
    {
        Lodding = true;
        var request = new GetStaffItemsRequest(PageIndex, PageSize, Search, Enabled);
        var reponse = await AuthClient.GetStaffItemsAsync(request);
        if (reponse.Success)
        {
            Staffs = reponse.Data;
        }
        else OpenErrorMessage(T("Failed to query staff data !"));
        Lodding = false;
    }  

    public void OpenStaffDialog(StaffItemResponse staff)
    {
        CurrentStaffId = staff.StaffId;
        StaffDialog = true;
    }
}

