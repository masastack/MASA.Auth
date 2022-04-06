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

    private async Task UpdateVisible(bool visible)
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

    private async Task Checked(DepartmentDto department)
    {
        if(Value != department.Id)
        {
            Value = department.Id;
            await ValueChanged.InvokeAsync(department);
            await UpdateVisible(false);
        }
    }
}

