namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class DepartmentDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public Guid Value { get; set; }

    [Parameter]
    public EventCallback<DepartmentDto> ValueChanged { get; set; }

    private string? Search { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    private bool IsFirst { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsFirst && Value != default)
        {
            var department = FindDepartment(Departments);
            if (department is not null)
            {
                await ValueChanged.InvokeAsync(department);
            }
            IsFirst = false;
        }

        DepartmentDto? FindDepartment(List<DepartmentDto> departments)
        {
            var department = departments.FirstOrDefault(d => d.Id == Value);
            if(department is not null) return department;
            else
            {
                foreach(var item in departments)
                {
                    var children = FindDepartment(item.Children);
                    if(children is not null) return children;
                }
            }
            return null;
        }
    }

    private async Task UpdateVisibleAsync(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    private async Task CheckedAsync(DepartmentDto department)
    {
        if (Value != department.Id)
        {
            Value = department.Id;
            await ValueChanged.InvokeAsync(department);
            await UpdateVisibleAsync(false);
        }
    }
}

