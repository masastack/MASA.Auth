namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class OrgSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public AddOrUpdateDepartmentDto Dto { get; set; } = new();

    [Parameter]
    public EventCallback<AddOrUpdateDepartmentDto> OnSubmit { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();


    public async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(Dto);
        }
    }

    public async Task OnDeleteHandler()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(Dto.Id);
        }
    }
}