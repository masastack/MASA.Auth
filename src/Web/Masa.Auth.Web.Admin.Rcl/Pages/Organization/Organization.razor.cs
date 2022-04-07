using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class Organization
{
    List<Guid> _active = new List<Guid>();
    Guid _currentStaffId = Guid.Empty;
    List<DepartmentDto> _departments = new();
    bool _showAdd, _showCopy, _addStaff, _updateStaff;
    DepartmentChildrenCountDto _departmentChildrenCountDto = new();
    readonly List<DataTableHeader<StaffDto>> _headers = new()
    {
        new() { Text = "员工", Value = nameof(StaffDto.Name), CellClass = "text-body neutral-lighten-1--text" },
        new() { Text = "手机号", Value = nameof(StaffDto.PhoneNumber), CellClass = "text-body3" },
        new() { Text = "邮箱", Value = nameof(StaffDto.Email), CellClass = "text-body3" },
        new() { Text = "工号", Value = nameof(StaffDto.JobNumber), CellClass = "text-body3" },
        new() { Text = "操作", Value = "Action", Sortable = false, Width = 80 }
    };
    PaginationDto<StaffDto> _paginationStaffs = new();
    UpsertDepartmentDto _upsertDepartmentDto = new();
    CopyDepartmentDto _copyDepartmentDto = new();
    GetStaffsDto _getStaffsDto = new GetStaffsDto(1, 10, "", Guid.Empty);
    DepartmentService DepartmentService => AuthCaller.DepartmentService;
    StaffService StaffService => AuthCaller.StaffService;

    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDepartmentsAsync();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadDepartmentsAsync()
    {
        _departments = await DepartmentService.GetListAsync();
        _departmentChildrenCountDto = await DepartmentService.GetCountAsync();
    }

    private async Task LoadStaffsAsync(Guid departmentId)
    {
        _getStaffsDto.DepartmentId = departmentId;
        var data = await StaffService.GetStaffsAsync(_getStaffsDto);
        _paginationStaffs = data;
    }

    private async Task LoadStaffsAsync()
    {
        var data = await StaffService.GetStaffsAsync(_getStaffsDto);
        _paginationStaffs = data;
    }

    private void Add(Guid parentId)
    {
        _upsertDepartmentDto = new UpsertDepartmentDto();
        _upsertDepartmentDto.ParentId = parentId;
        _showAdd = true;
    }

    private async Task EnterSearch(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == Keyboards.Enter)
        {
            await LoadStaffsAsync();
        }
    }

    private async Task DeleteAsync(Guid departmentId)
    {
        await DepartmentService.RemoveAsync(departmentId);
        await LoadDepartmentsAsync();
        _showAdd = false;
    }

    private async Task Update(Guid departmentId)
    {
        var department = await DepartmentService.GetAsync(departmentId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _upsertDepartmentDto = new UpsertDepartmentDto();
        _upsertDepartmentDto.Id = department.Id;
        _upsertDepartmentDto.Name = department.Name;
        _upsertDepartmentDto.Description = department.Description;
        _upsertDepartmentDto.Enabled = department.Enabled;
        _upsertDepartmentDto.ParentId = department.ParentId;
        _showAdd = true;
    }

    private async Task SubmitAsync(UpsertDepartmentDto dto)
    {
        await DepartmentService.UpsertAsync(dto);
        await LoadDepartmentsAsync();
        _showAdd = false;
    }

    private async Task SubmitAsync(CopyDepartmentDto dto)
    {
        await DepartmentService.UpsertAsync(dto);
        await LoadDepartmentsAsync();
        _showCopy = false;
    }

    private async Task Copy(Guid sourceId)
    {
        var department = await DepartmentService.GetAsync(sourceId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _copyDepartmentDto = new CopyDepartmentDto();
        _copyDepartmentDto.Name = department.Name;
        _copyDepartmentDto.Description = department.Description;
        _copyDepartmentDto.Enabled = department.Enabled;
        _copyDepartmentDto.ParentId = department.ParentId;
        _copyDepartmentDto.Staffs = department.StaffList;
        _showCopy = true;
    }

    private async Task ActiveUpdated(List<DepartmentDto> activedItems)
    {
        if (!activedItems.Any())
        {
            return;
        }
        await LoadStaffsAsync(activedItems.First().Id);
    }

    private async Task UpdateStaff(Guid staffId)
    {
        _currentStaffId = staffId;
        _updateStaff = true;
    }
}

