namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleSelectForUser : RoleSelect
{
    [Parameter]
    public Guid UserId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Roles = await RoleService.GetSelectForUserAsync(UserId);
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity <= 0;
}

