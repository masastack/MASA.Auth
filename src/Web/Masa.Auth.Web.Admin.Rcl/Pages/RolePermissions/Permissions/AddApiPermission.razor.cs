namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddApiPermission
{
    [EditorRequired]
    [Parameter]
    public List<AppDto> AppItems { get; set; } = new();

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<ApiPermissionDetailDto> OnSubmit { get; set; }

    [EditorRequired]
    [Parameter]
    public List<SelectItemDto<PermissionTypes>> PermissionTypes { get; set; } = new();

    ApiPermissionDetailDto _apiPermissionDetailDto = new();

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(_apiPermissionDetailDto);
        }
    }
}
